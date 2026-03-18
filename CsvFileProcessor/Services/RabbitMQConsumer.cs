using CsvFileProcessor.Services.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvFileProcessor.Services
{
	public class RabbitMQConsumer
	{
		private readonly IFileProcessor _fileProcessor;

		public RabbitMQConsumer(IFileProcessor fileProcessor)
		{
			_fileProcessor = fileProcessor;
		}
		public async Task StartAsync()
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

			Console.WriteLine("Waiting for messages...");

			var consumer = new AsyncEventingBasicConsumer(channel);

			consumer.ReceivedAsync += async (sender, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);

				Console.WriteLine($"File Received");

				await _fileProcessor.ProcessFile(message);
			};

			await channel.BasicConsumeAsync(
				queue: "file_queue",
				autoAck: true,
				consumer: consumer);

			
			Console.ReadLine();
		}
	}
}

