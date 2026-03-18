using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.UserModule.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Services.UserModule.Interfaces
{
	public interface IUserService
	{
		Task<User> UserLoginAsync(string Email, string Password);
		Task<bool> UserRegistrationAsync(RegisrationDTO user);

	}
}
