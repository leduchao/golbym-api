using golbym.Api.Models.Dtos;
using golbym.Api.Services;
using System.IdentityModel.Tokens.Jwt;

namespace golbym.Api.Endpoints
{
	public static class AuthEndpoints
	{
		public static void MapAuthEndpoint(this IEndpointRouteBuilder app)
		{
			var group = app.MapGroup("api/v1/auth").WithTags("Auth");

			group.MapPost("login", Login);
			group.MapPost("signup", SignUp);
			group.MapGet("user-info/{userName}", GetUserInfo).RequireAuthorization();
			group.MapGet("username", GetUserName).RequireAuthorization();
		}

		private static async Task<IResult> Login(LoginModel model, IAuthService authService)
		{
			var response = await authService.Login(model);

			if (string.IsNullOrEmpty(response.Token))
				return TypedResults.BadRequest(response);

			return TypedResults.Ok(response);
		}

		private static async Task<IResult> SignUp(SignUpModel model, IAuthService authService)
		{
			var result = await authService.SignUp(model);

			if (result.Succeeded)
				return TypedResults.Ok(result);

			return TypedResults.BadRequest(result);
		}

		private static async Task<IResult> GetUserInfo(IAuthService authService, string userName)
		{
			var user = await authService.GetUserInfo(userName);

			if (user is null)
				return TypedResults.NotFound("Khong tim thay user");

			return TypedResults.Ok(user);
		}

		private static async Task<IResult> GetUserName(IAuthService authService, HttpContext context)
		{
			var userName = context.User.Claims
				.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value ?? "uname";

			var user = await authService.GetUserInfo(userName);

			if (user is null)
				return TypedResults.NotFound("Khong tim thay user");

			return TypedResults.Ok(userName);
		}
	}
}
