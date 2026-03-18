using System.ComponentModel.DataAnnotations;

namespace CSVFileApplication.API.UserModule.DTOs
{
	public class LoginRequests
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
