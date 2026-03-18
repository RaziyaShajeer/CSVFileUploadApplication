using CsvFileProcessor.Services.DTOs;
using CsvFileProcessor.Services.Interface;
using CsvFileuploadDomain.Models;
using CsvHelper;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileProcessor.Services
{
	public class FileProcessor:IFileProcessor
	{
		private readonly UserDbContext _context;

		public FileProcessor(UserDbContext context)
		{
			_context = context;
		}
		public async Task ProcessFile(string fileId)
		{
			var connection = new HubConnectionBuilder()
	 .WithUrl("https://localhost:7122/fileProcessingHub") 
	 .WithAutomaticReconnect() 
	 .Build();
			try
			{
				// Start the connection
				await connection.StartAsync();
				Console.WriteLine("Connected to SignalR hub");
				int convertedId = int.Parse(fileId);

				var fileProcess = await _context.FileProcesses.FindAsync(convertedId);

				if (fileProcess == null)
					return;

				int rowNumber = fileProcess.LastProcessedRow;

				using var reader = new StreamReader(fileProcess.FilePath);
				using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

				csv.Context.Configuration.HasHeaderRecord = false;

				int currentRow = 0;

				foreach (var record in csv.GetRecords<CsvRow>())
				{
					currentRow++;

					// Skip already processed rows
					if (currentRow <= rowNumber)
						continue;

					var data = new CsvFileData
					{
						Name = record.Name,
						Email = record.Email
					};

					await _context.CsvFilesDatas.AddAsync(data);

					rowNumber++;

					// Change status when processing starts
					if (rowNumber == 1)
					{
						fileProcess.Status = "Processing";

						await _context.SaveChangesAsync();

						await connection.InvokeAsync("SendStatusUpdate",fileProcess.Id,"Processing",fileProcess.ProcessedRows,fileProcess.FilePath
);
					
					}

					// Save progress every 100 rows
					if (rowNumber % 10 == 0)
					{
						fileProcess.ProcessedRows = rowNumber;
						fileProcess.LastProcessedRow = rowNumber;

						await _context.SaveChangesAsync();
					}

				}

				// Final update after all rows processed
				fileProcess.Status = "Completed";
				fileProcess.ProcessedRows = rowNumber;
				fileProcess.LastProcessedRow = rowNumber;

				await _context.SaveChangesAsync();
				await connection.InvokeAsync("SendStatusUpdate",fileProcess.Id,"Completed",fileProcess.ProcessedRows,fileProcess.FilePath);
				await Task.Delay(100);

			}
			catch (Exception ex)
			{
				// Mark failed if error occurs
				var convertedId = int.Parse(fileId);
				var fileProcess = await _context.FileProcesses.FindAsync(convertedId);

				if (fileProcess != null)
				{
					fileProcess.Status = "Failed";
					await _context.SaveChangesAsync();
					await connection.InvokeAsync("SendStatusUpdate", fileProcess.Id,"Failed",fileProcess.ProcessedRows,fileProcess.FilePath);
				
                }
			}
		}

	}
	}

