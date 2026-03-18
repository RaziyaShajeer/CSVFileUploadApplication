using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Services.RabitMqService.Interface
{
	public interface IRabbitMQPublisher
	{
		Task SendMessage(int filePath);
	}
}
