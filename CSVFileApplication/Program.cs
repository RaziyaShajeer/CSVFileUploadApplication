
using CSVFileApplication.Extensions;
using CSVFileApplication.SignalRHub;
using CsvFileuploadDomain.Models;
using CsvFileuploadDomain.Services.RabitMqService;
using CsvFileuploadDomain.Services.RabitMqService.Interface;
using CsvFileuploadDomain.Services.UserModule;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using Domain.Services.DashbordModule;
using Domain.Services.DashbordModule.Interface;
using Domain.Services.UserModule.Interface;
using Microsoft.EntityFrameworkCore;
using Unity;
using Unity.Microsoft.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddDbContext<UserDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

IUnityContainer container = new UnityContainer();
container.RegisterType<IUserService, UserService>();
container.RegisterType<IUserRepository, UserRepository>();
container.RegisterType<IDashboardRepository, DashboardRepository>();
container.RegisterType<IRabbitMQPublisher,RabbitMQPublisherService>();
container.RegisterType<IDashboardService,Dashboardservice>();   
// Plug Unity into ASP.NET Core's DI pipeline
builder.Host.UseUnityServiceProvider(container);
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
var app = builder.Build();
app.MapHub<FileProcessingHub>("/fileProcessingHub");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();   
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.Run();
