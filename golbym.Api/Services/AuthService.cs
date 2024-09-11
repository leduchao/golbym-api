using golbym.Api.Domains;
using golbym.Api.Models.ApiResponse;
using golbym.Api.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace golbym.Api.Services
{
	public class AuthService(
		UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager,
		JwtService jwtService) : IAuthService
	{
		public async Task<AppUser?> GetUserInfo(string userName)
		{
			return await userManager.FindByNameAsync(userName);
		}

		public async Task<LoginResponse> Login(LoginModel model)
		{
			var user = await userManager.FindByNameAsync(model.Username);


			if (user is not null)
			{
				var validPassword = await userManager.CheckPasswordAsync(user, model.Password);

				if (validPassword)
				{
					var token = await jwtService.GenerateJwtAsync(user);

					return new LoginResponse
					{
						Token = token,
						Result = IdentityResult.Success
					};
				}
			}

			return new LoginResponse
			{
				Token = "",
				Result = IdentityResult.Failed(new IdentityError
				{
					Code = "InvalidCredentials",
					Description = "The username or password you entered is incorrect. Please try again."
				})
			};
		}

		public async Task<IdentityResult> SignUp(SignUpModel model)
		{
			var existEmail = await userManager.FindByEmailAsync(model.Email);

			if (existEmail is not null)
			{
				return IdentityResult.Failed(new IdentityError
				{
					Code = "ResourceConflict",
					Description = "Email already exits!",
				});
			}

			var newUser = new AppUser
			{
				UserName = model.Username,
				Email = model.Email,
			};

			var result = await userManager.CreateAsync(newUser, model.Password);

			if (!result.Succeeded)
			{
				var errorsList = result.Errors.ToArray();
				return IdentityResult.Failed(errorsList);
			}

			await AddRoleAsync(newUser);
			return result;
		}

		private async Task AddRoleAsync(AppUser newUser)
		{

			// neu da co role admin trong csdl
			if (await roleManager.RoleExistsAsync(Role.Admin))
			{
				// lay danh sach user co role admin
				var admin = await userManager.GetUsersInRoleAsync("Admin");

				// neu chua co ai thuoc role admin
				if (admin is null || admin.Count == 0)
				{
					await userManager.AddToRoleAsync(newUser, Role.Admin);
				}
				else
				{
					// neu chua co role Viewer trong csdl
					if (!await roleManager.RoleExistsAsync(Role.Viewer))
					{
						// tao role Viewer
						await roleManager.CreateAsync(new IdentityRole(Role.Viewer));
					}

					await userManager.AddToRoleAsync(newUser, Role.Viewer);
				}
			}
			else
			{
				// tao role admin va them user vao
				await roleManager.CreateAsync(new IdentityRole(Role.Admin));
				await userManager.AddToRoleAsync(newUser, Role.Admin);
			}

		}
	}
}
