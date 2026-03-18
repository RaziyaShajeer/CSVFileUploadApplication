using AutoMapper;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.DashbordModule.DTOs;
using Domain.Services.DashbordModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.DashbordModule
{
	public class Dashboardservice:IDashboardService
	{
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IMapper _mapper;
        public Dashboardservice(IDashboardRepository dashboardRepository,IMapper mapper)
        {
            _dashboardRepository= dashboardRepository;
            _mapper= mapper;
        }
        public async Task<List<FileViewDTO>> GetAllFiles()
        {
            var fileList=await _dashboardRepository.GetAllFiles();
            var fileDtos=_mapper.Map<List<FileViewDTO>>(fileList);
            return fileDtos;

        }

    }
}
