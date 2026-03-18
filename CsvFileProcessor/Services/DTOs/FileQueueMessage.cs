using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileProcessor.Services.DTOs
{
	public  class FileQueueMessage
	{
		public int FileId { get; set; }
		public string FilePath { get; set; }
	}
}
