using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Models
{
	public class FileProcess
	{
		public int Id { get; set; }

		public string FilePath { get; set; }

		public string Status { get; set; }

		public int ProcessedRows { get; set; }

		public int TotalRows { get; set; }
		public int LastProcessedRow { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime CreatedDate { get; set; }

	
	}
}
