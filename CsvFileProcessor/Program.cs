using CsvFileProcessor.Services;
using CsvFileProcessor.Services.Interface;
using CsvFileuploadDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Unity;

var builder = Host.CreateApplicationBuilder(args);

// 🔹 Unity container
var container = new UnityContainer();

container.RegisterFactory<UserDbContext>(c =>
{
    var options = new DbContextOptionsBuilder<UserDbContext>()
        .UseSqlServer("Data Source=DESKTOP-PBRNQVI;Initial Catalog=CSVFileData;User ID=myuser;Password=123;Trust Server Certificate=True")
        .Options;

    return new UserDbContext(options);
});

container.RegisterType<RabbitMQConsumer>();
container.RegisterType<IFileProcessor, FileProcessor>();

// ✅ Build ONLY ONCE
var host = builder.Build();

// 🔹 Resolve and start consumer
var consumer = container.Resolve<RabbitMQConsumer>();
await consumer.StartAsync();

// 🔹 Run host
await host.RunAsync();