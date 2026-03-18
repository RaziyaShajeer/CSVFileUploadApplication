using AutoMapper;
using CSVFileApplication.API.DashBoard;
using CSVFileApplication.API.UserModule;
using CSVFileApplication.SignalRHub;
using CsvFileuploadDomain.Enums;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.DashbordModule.DTOs;
using CsvFileuploadDomain.Services.RabitMqService.Interface;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using Domain.Services.DashbordModule.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFileApplicationTest
{
	public class DashBoardControllerTest
	{
		private DashboardController GetController(Mock<IDashboardRepository> dashBoradRepository,Mock<IDashboardService> dashBoardService, Mock<IRabbitMQPublisher> rabitmqPublisher)
		{
			var mapper = new Mock<IMapper>();

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();

			var accessor = new HttpContextAccessor
			{
				HttpContext = httpContext
			};

			return new DashboardController(
				dashBoradRepository.Object,
				rabitmqPublisher.Object,
				dashBoardService.Object,
				null
			);
		}
		[Fact]
		public async Task Dashboard_NoSession_RedirectsToLogin()
		{
			// Arrange
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession(); // empty session

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var result = await controller.DashBoard();

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Login", redirectResult.ActionName);
			Assert.Equal("Authentication", redirectResult.ControllerName);
		}
		[Fact]
		public async Task Dashboard_WithSession_ReturnsViewWithFiles()
		{
			// Arrange
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();

			var fileList = new List<FileViewDTO>
	{
		new FileViewDTO { Id = 1, FilePath = "C:\\Users\\Admin\\Downloads\\mynew.csv" ,Status=FileStatus.Started.ToString()}
	};

			repo.Setup(x => x.FindUnCompletedFiles())
				.Returns(Task.CompletedTask);

			service.Setup(x => x.GetAllFiles())
				   .ReturnsAsync(fileList);

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();
			httpContext.Session.SetString("UserId", "1");

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var result = await controller.DashBoard();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal(fileList, viewResult.Model);
		}
		[Fact]
		public async Task DashboardPost_NoSession_RedirectToLogin()
		{
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			var result = await controller.DashBoard("test.csv");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Login", redirect.ActionName);
		}

		[Fact]
		public async Task DashboardPost_EmptyFilePath_ReturnsView()
		{
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();
			httpContext.Session.SetString("UserId", "1");

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			var result = await controller.DashBoard("");

			var viewResult = Assert.IsType<ViewResult>(result);
		}
		[Fact]
		public async Task DashboardPost_FileNotExist_RedirectDashboard()
		{
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();
			httpContext.Session.SetString("UserId", "1");

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			var result = await controller.DashBoard("C:\\wrongfile.csv");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("DashBoard", redirect.ActionName);
		}
		[Fact]
		public async Task DashboardPost_FileAdded_SendsRabbitMQ()
		{
			var repo = new Mock<IDashboardRepository>();
			var publisher = new Mock<IRabbitMQPublisher>();
			var service = new Mock<IDashboardService>();
			var hub = new Mock<IHubContext<FileProcessingHub>>();
			var clients = new Mock<IHubClients>();
			var clientProxy = new Mock<IClientProxy>();

			hub.Setup(x => x.Clients).Returns(clients.Object);
			clients.Setup(x => x.All).Returns(clientProxy.Object);

			repo.Setup(x => x.AddFileName(It.IsAny<FileProcess>()))
				.ReturnsAsync(1);

			var controller = new DashboardController(
				repo.Object,
				publisher.Object,
				service.Object,
				hub.Object
			);

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();
			httpContext.Session.SetString("UserId", "1");

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			var filePath = Path.GetTempFileName();

			var result = await controller.DashBoard(filePath);

			publisher.Verify(x => x.SendMessage(It.IsAny<int>()), Times.Once);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
		}
	}

	}

