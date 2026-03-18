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
            var convertedId = int.Parse(fileId);

            var fileProcess = await _context.FileProcesses.FindAsync(convertedId);
            if (fileProcess == null)
                return;

            // Create connection ONCE
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7122/fileProcessingHub")
                .WithAutomaticReconnect()
                .Build();

            await connection.StartAsync();

            try
            {
                int lastProcessedRow = fileProcess.LastProcessedRow;

                using var reader = new StreamReader(fileProcess.FilePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Context.Configuration.HasHeaderRecord = false;

                var records = csv.GetRecords<CsvRow>().Skip(lastProcessedRow);

                int currentRow = lastProcessedRow;

                foreach (var record in records)
                {
                    var data = new CsvFileData
                    {
                        Name = record.Name,
                        Email = record.Email
                    };

                    await _context.CsvFilesDatas.AddAsync(data);

                    currentRow++;

                    // Update tracking
                    fileProcess.LastProcessedRow = currentRow;
                    fileProcess.ProcessedRows = currentRow;
                    fileProcess.Status = "Processing";

                    // Save every 10 rows
                    if (currentRow % 10 == 0)
                    {
                        await _context.SaveChangesAsync();

                        await connection.InvokeAsync(
                            "SendStatusUpdate",
                            fileProcess.Id,
                            "Processing",
                            fileProcess.ProcessedRows,
                            fileProcess.FilePath
                        );
                    }
                }

                // Final save
                fileProcess.Status = "Completed";

                await _context.SaveChangesAsync();

                await connection.InvokeAsync(
                    "SendStatusUpdate",
                    fileProcess.Id,
                    "Completed",
                    fileProcess.ProcessedRows,
                    fileProcess.FilePath
                );
            }
            catch (Exception ex)
            {
                fileProcess.Status = "Failed";

                await _context.SaveChangesAsync();

                await connection.InvokeAsync(
                    "SendStatusUpdate",
                    fileProcess.Id,
                    "Failed",
                    fileProcess.ProcessedRows,
                    fileProcess.FilePath
                );
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }

    }
	}

