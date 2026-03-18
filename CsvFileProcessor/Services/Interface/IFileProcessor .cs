using CsvFileuploadDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileProcessor.Services.Interface
{
	public interface IFileProcessor
	{
		Task ProcessFile(string filePath);
	}
}
