using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Exceptions;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services.UserModule.Interface;
using Microsoft.EntityFrameworkCore;

namespace CsvFileuploadDomain.Services.UserModule
{
	public class UserRepository:IUserRepository
	{
		private readonly UserDbContext _context;
		public UserRepository(UserDbContext userDbContext)
		{
			_context = userDbContext;
		}
		public async Task<bool> UserRegistrationAsync(User user)
		{
			var exitingUser= _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();
			if (exitingUser == null) { 
			await _context.Users.AddAsync(user);	
				await _context.SaveChangesAsync();
				return true;	
			}
			else
			{
			
				return false;
			}


		}
		public async Task<User> UserLoginAsync(string Email, string Password)
		{
			var user = await _context.Users.Where(e => e.Email == Email && e.Password == Password).FirstOrDefaultAsync();
			if (user != null)
			{
				return user;
			}

			return null;
		}
	}
}
