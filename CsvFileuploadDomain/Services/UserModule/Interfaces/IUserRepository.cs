
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvFileuploadDomain.Models;
namespace Domain.Services.UserModule.Interface
{
	public interface IUserRepository
	{
		Task<bool> UserRegistrationAsync(User user);
		Task<User> UserLoginAsync(string Email, string Password);


	}
}
