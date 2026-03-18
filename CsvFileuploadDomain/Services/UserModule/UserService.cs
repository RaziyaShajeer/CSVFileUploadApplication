using AutoMapper;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.UserModule.DTOs;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using Domain.Services.UserModule.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Services.UserModule
{
	public class UserService:IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		public UserService(IUserRepository userRepository,IMapper mapper) {
		_userRepository = userRepository;	
			_mapper=mapper;
		}
		public async  Task<bool> UserRegistrationAsync(RegisrationDTO userDto)
		{
			var user=_mapper.Map<User>(userDto);
			
			return await _userRepository.UserRegistrationAsync(user);

		}

		public async Task<User> UserLoginAsync(string Email, string Password)
		{

			var result = await _userRepository.UserLoginAsync(Email, Password);
			return result;
		}
	}
}
