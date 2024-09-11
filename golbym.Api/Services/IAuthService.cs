using golbym.Api.Domains;
using golbym.Api.Models.ApiResponse;
using golbym.Api.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace golbym.Api.Services
{
	public interface IAuthService
	{
		Task<IdentityResult> SignUp(SignUpModel model);

		Task<LoginResponse> Login(LoginModel model);

		Task<AppUser?> GetUserInfo(string userName);
	}
}
