using AutoMapper;
using CSVFileApplication.API.UserModule.DTOs;
using CsvFileuploadDomain.Services.UserModule.DTOs;
using CsvFileuploadDomain.Services.UserModule.Interfaces;
using Domain.Services.DashbordModule.Interface;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CSVFileApplication.API.UserModule
{
	public class AuthenticationController : Controller
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;	
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDashboardRepository _dashboardRepository;
		public AuthenticationController(IMapper mapper,IUserService userService, IWebHostEnvironment environment,IHttpContextAccessor httpContextAccessor, IDashboardRepository dashboardRepository)
		{
			_mapper=mapper;
			_userService=userService;
			_httpContextAccessor=httpContextAccessor;
			_dashboardRepository = dashboardRepository;
		}
		public IActionResult Index()
		{
			return View();

		}
		[HttpGet]
		public IActionResult SignUp()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
		{
			var user = _mapper.Map<RegisrationDTO>(signUpRequest);
			var result=await _userService.UserRegistrationAsync(user);
			if(result)
			return RedirectToAction("Login");
			else
				return View();

		}
		[HttpGet]
		public async Task<IActionResult> Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginRequests loginRequest)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = await _userService.UserLoginAsync(loginRequest.Email, loginRequest.Password);

					if (user != null)
					{
						_httpContextAccessor.HttpContext.Session.SetString("UserName", user.Name);
						_httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id.ToString());
						await _dashboardRepository.FindUnCompletedFiles();
						// Login success
						return RedirectToAction("DashBoard", "Dashboard");
					}
					else
					{
						ModelState.AddModelError("", "Invalid Email or Password");
					}
				}

				return View(loginRequest);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "An error occurred while processing your request.");
				return View(loginRequest);
			}
		}
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();   // remove all session values
			return RedirectToAction("Login", "Authentication");
		}
	}
}
