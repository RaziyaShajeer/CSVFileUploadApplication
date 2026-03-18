using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Exceptions
{
    public class UserAlreadyExistException:Exception
    {
        public UserAlreadyExistException() { }
        public UserAlreadyExistException(string message) : base(message) { }
    }
}
