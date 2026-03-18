using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Models
{
	public class UserDbContext:DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<CsvFileData> CsvFilesDatas { get; set; }
		public DbSet<FileProcess> FileProcesses { get; set; }	

		public UserDbContext(DbContextOptions<UserDbContext> options)
		  : base(options)
		{
		}

	}
}
