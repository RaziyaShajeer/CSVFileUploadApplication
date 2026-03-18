using CsvFileuploadDomain.Services.RabitMqService.Interface;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileuploadDomain.Services.RabitMqService
{
	public class RabbitMQPublisherService : IRabbitMQPublisher
	{
		public async Task SendMessage(int filePath)
		{
			var factory = new ConnectionFactory()
			{
				HostName = "localhost"
			};
			await using var connection = await factory.CreateConnectionAsync();
			await using var channel = await connection.CreateChannelAsync();

			await channel.QueueDeclareAsync(
		queue: "file_queue",
		durable: false,
		exclusive: false,
		autoDelete: false,
		arguments: null);
			
			Console.WriteLine("File Added To Queue");
			var body = Encoding.UTF8.GetBytes(filePath.ToString());



			await channel.BasicPublishAsync(
				exchange: "",
				routingKey: "file_queue",
				body: body);
			Console.WriteLine("File AddedTo Queue");
			await Task.Delay(2000);
		}

	}
	}


