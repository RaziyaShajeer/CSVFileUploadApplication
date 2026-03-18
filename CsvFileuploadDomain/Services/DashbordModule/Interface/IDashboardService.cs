using CsvFileuploadDomain.Services.DashbordModule.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.DashbordModule.Interface
{
	public interface IDashboardService
	{
        Task<List<FileViewDTO>> GetAllFiles();

    }
}
