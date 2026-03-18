using AutoMapper;
using CSVFileApplication.API.UserModule;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.DashbordModule.DTOs;
using CsvFileuploadDomain.Services.UserModule.DTOs;




namespace CSVFileApplication.Extensions
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<SignUpRequest,RegisrationDTO>().ReverseMap();	
			CreateMap<RegisrationDTO,User>().ReverseMap();	
			CreateMap<FileProcess,FileViewDTO>().ReverseMap();	
			
		}
	}
}

