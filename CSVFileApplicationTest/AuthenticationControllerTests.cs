using AutoMapper;
using CSVFileApplication.API.UserModule;
using CSVFileApplication.API.UserModule.DTOs;

using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.UserModule;
using CsvFileuploadDomain.Services.UserModule.DTOs;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSVFileApplicationTest
{
	public class AuthenticationControllerTests
	{

		private AuthenticationController GetController(Mock<IUserService> userServiceMock)
		{
			var mapper = new Mock<IMapper>();

			var httpContext = new DefaultHttpContext();
			httpContext.Session = new DummySession();

			var accessor = new HttpContextAccessor
			{
				HttpContext = httpContext
			};

			return new AuthenticationController(
				mapper.Object,
				userServiceMock.Object,
				null,
				accessor
			);
		}
		[Fact]
		public async Task Login_ValidUser_RedirectsToDashboard()
		{
			var userService = new Mock<IUserService>();

			userService.Setup(x => x.UserLoginAsync("test@gmail.com", "123"))
					   .ReturnsAsync(new User
					   {
						   Email = "test@gmail.com",
						   Name = "TestUser"
					   });

			var controller = GetController(userService);

			var request = new LoginRequests
			{
				Email = "test@gmail.com",
				Password = "123"
			};

			var result = await controller.Login(request);

			var redirect = Assert.IsType<RedirectToActionResult>(result);

			Assert.Equal("DashBoard", redirect.ActionName);
			Assert.Equal("Dashboard", redirect.ControllerName);
		}
		[Fact]
		public async Task Login_InvalidUser_ReturnsView()
		{
			var userService = new Mock<IUserService>();

			userService.Setup(x => x.UserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
					   .ReturnsAsync((User)null);

			var controller = GetController(userService);

			var request = new LoginRequests
			{
				Email = "wrong@gmail.com",
				Password = "111"
			};

			var result = await controller.Login(request);

			var view = Assert.IsType<ViewResult>(result);

			Assert.Equal(request, view.Model);
		}
		[Fact]
		public async Task SignUp_ValidRequest_RedirectsToLogin()
		{
			// Arrange
			var userService = new Mock<IUserService>();
			var mapper = new Mock<IMapper>();

			var signUpRequest = new SignUpRequest
			{
				Name = "Rasiya",
				Email = "razzrasiya@gmail.com",
				Password = "123"
			};

			var registrationDto = new RegisrationDTO
			{
				Name = "Rasiya",
				Email = "razzrasiya@gmail.com",
				Password = "123"
			};

			// Mock mapper
			mapper.Setup(x => x.Map<RegisrationDTO>(signUpRequest))
				  .Returns(registrationDto);

			// Mock service
			userService.Setup(x => x.UserRegistrationAsync(registrationDto))
					   .ReturnsAsync(true);

			var controller = new AuthenticationController(
				mapper.Object,
				userService.Object,
				null,
				new HttpContextAccessor()
			);

			// Act
			var result = await controller.SignUp(signUpRequest);

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Login", redirectResult.ActionName);
		}
		[Fact]
		public async Task SignUp_InvalidRequest_ReturnsView()
		{
			// Arrange
			var userService = new Mock<IUserService>();
			var mapper = new Mock<IMapper>();

			var signUpRequest = new SignUpRequest
			{
				Name = "Rasiya",
				Email = "razzrasiya@gmail.com",
				Password = "123"
			};

			var registrationDto = new RegisrationDTO
			{
				Name = "Rasiya",
				Email = "razzrasiya@gmail.com",
				Password = "123"
			};

			// Mock mapper
			mapper.Setup(x => x.Map<RegisrationDTO>(signUpRequest))
				  .Returns(registrationDto);

			// Mock service → signup fails
			userService.Setup(x => x.UserRegistrationAsync(registrationDto))
					   .ReturnsAsync(false);

			var controller = new AuthenticationController(
				mapper.Object,
				userService.Object,
				null,
				new HttpContextAccessor()
			);

			// Act
			var result = await controller.SignUp(signUpRequest);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
		}
	}
}
