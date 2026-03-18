using CSVFileApplication.SignalRHub;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.RabitMqService.Interface;
using Domain.Services.DashbordModule.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CSVFileApplication.API.DashBoard
{
	public class DashboardController : Controller
	{
		private readonly IDashboardRepository _dashboardRepository;
		private readonly IRabbitMQPublisher _rabbitMQPublisher;
		private readonly IDashboardService _dashboardService;
		private readonly IHubContext<FileProcessingHub> _hubContext;
		public DashboardController(IDashboardRepository dashboardRepository, IRabbitMQPublisher rabbitMQPublisher, IDashboardService dashboardService, IHubContext<FileProcessingHub> hubContext)
		{
			_dashboardRepository = dashboardRepository;
			_rabbitMQPublisher = rabbitMQPublisher;
			_dashboardService = dashboardService;
			_hubContext = hubContext;
		}
		public async Task<IActionResult> DashBoard()
		{
			//  Check session first
			if (HttpContext.Session.GetString("UserId") == null)
			{
				// Session is null → redirect to login
				return RedirectToAction("Login", "Authentication");
			}
		
			var filelist = await _dashboardService.GetAllFiles();
			return View(filelist);
		}
		[HttpPost]
		public async Task<IActionResult> DashBoard(string filePath)
		{
			try
			{
				if (HttpContext.Session.GetString("UserId") == null)
				return RedirectToAction("Login", "Authentication");

			if (string.IsNullOrWhiteSpace(filePath))
			{
				ViewBag.Message = "Invalid file path.";
					var filelist = await _dashboardService.GetAllFiles();
					return View(filelist);
			}
			if (!System.IO.File.Exists(filePath))
			{
				ViewBag.Message = "File does not exist.";
					var filelist = await _dashboardService.GetAllFiles();
					return RedirectToAction("filelist");
			}

			
				var file = new FileProcess
				{
					FilePath = filePath,
					Status = "Started",
					ProcessedRows = 0,
					LastProcessedRow = 0,
					CreatedDate = DateTime.Now
				};

				var addedFileId = await _dashboardRepository.AddFileName(file);

				if (addedFileId == null)
				{
					var filelist = await _dashboardService.GetAllFiles();
					ViewBag.Message = $"File '{file.FilePath}' is already in the system.";
					return View(filelist);
				}
				else
				{
					await _hubContext.Clients.All.SendAsync(
						"ReceiveStatusUpdate",
						file.Id,
						file.Status,
						file.ProcessedRows,
						file.FilePath
					);

					await _rabbitMQPublisher.SendMessage(file.Id);
				}

				return RedirectToAction("DashBoard");
			}
			catch (Exception)
			{
				return RedirectToAction("DashBoard");
			}
		}
	}
}
