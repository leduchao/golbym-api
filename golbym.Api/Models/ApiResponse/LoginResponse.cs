using Microsoft.AspNetCore.Identity;

namespace golbym.Api.Models.ApiResponse
{
	public class LoginResponse
	{
		public IdentityResult Result { get; set; } = new();
		public string Token { get; init; } = string.Empty;
	}
}
