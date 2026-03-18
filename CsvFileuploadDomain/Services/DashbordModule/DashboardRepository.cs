using CsvFileuploadDomain.Enums;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.RabitMqService.Interface;
using Domain.Services.DashbordModule.Interface;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.DashbordModule
{
	public class DashboardRepository : IDashboardRepository
	{
		private readonly UserDbContext _context;
		private readonly IRabbitMQPublisher _rabbitMQPublisher;
		public DashboardRepository(UserDbContext context,IRabbitMQPublisher rabbitMQPublisher)
		{
			_context = context;
			_rabbitMQPublisher = rabbitMQPublisher;
		}
		public async Task<List<CsvFileData>> AddDataToDatabase(List<CsvFileData> data)
		{
			try
			{
				await _context.CsvFilesDatas.AddRangeAsync(data);
				await _context.SaveChangesAsync();
				return data;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		public async Task<int?> AddFileName(FileProcess fileProcess)
		{
			var isExist = await _context.FileProcesses
				.AnyAsync(e => e.FilePath == fileProcess.FilePath);

			if (!isExist)
			{
				await _context.FileProcesses.AddAsync(fileProcess);
				await _context.SaveChangesAsync();
				return fileProcess.Id; // new file added
			}
			else
			{
				return null; // file already exists
			}
		}
		public async Task FindUnCompletedFiles()
        {
			var unProcessedList = await _context.FileProcesses.Where(e => e.Status == FileStatus.Processing.ToString()||e.Status==FileStatus.Failed.ToString()||e.Status==FileStatus.Started.ToString()).ToListAsync();
            if (unProcessedList.Any())
            {
                foreach (var unProcessed in unProcessedList)
                {
                    await _rabbitMQPublisher.SendMessage(unProcessed.Id);
                }
            }

        }
		public async Task<List<FileProcess>> GetAllFiles()
		{
			var listOfFiles = await _context.FileProcesses.OrderByDescending(f => f.Id)   // newest first
		.ToListAsync(); 
			return listOfFiles;	
		}

    }
}
