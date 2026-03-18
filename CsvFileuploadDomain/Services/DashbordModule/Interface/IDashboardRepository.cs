using CsvFileuploadDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.DashbordModule.Interface
{
	public interface IDashboardRepository
	{
		Task<List<CsvFileData>> AddDataToDatabase(List<CsvFileData> data);
		Task<int?> AddFileName(FileProcess fileProcess);
        Task FindUnCompletedFiles();
		Task<List<FileProcess>> GetAllFiles();
    }
}
