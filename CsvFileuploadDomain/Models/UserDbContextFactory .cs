using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Models
{
	public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
	{
		public UserDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

			optionsBuilder.UseSqlServer(
				"Data Source=DESKTOP-PBRNQVI;Initial Catalog=CSVFileData;User ID=myuser;Password=123;Trust Server Certificate=True");

			return new UserDbContext(optionsBuilder.Options);
		}
	}
}
