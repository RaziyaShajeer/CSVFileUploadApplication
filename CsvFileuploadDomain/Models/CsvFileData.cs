using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Models
{
    public class CsvFileData
    {
        [Key]
        public int Id { get;set; }
        public string Name { get;set; }
        public string Email { get;set; }
    }
}
