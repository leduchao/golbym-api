using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using golbym.Api.Domains;

namespace golbym.Api.Services
{
	public class JwtService(IConfiguration config, UserManager<AppUser> usermanager)
	{
		public async Task<string> GenerateJwtAsync(AppUser user)
		{
			var securityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value!));

			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.Id ?? "User1"),
				new(JwtRegisteredClaimNames.Email, user.Email ?? "example@email.com"),
				new(JwtRegisteredClaimNames.Name, user.UserName ?? "username"),
			};

			var userRoles = await usermanager.GetRolesAsync(user);

			foreach(var role in userRoles)
			{
				claims.Add(new(ClaimTypes.Role, role));
			}

			var jwtSecurityToken = new JwtSecurityToken(
				config.GetSection("Jwt:Issuer").Value,
				config.GetSection("Jwt:Issuer").Value,
				claims,
				null,
				DateTime.Now.AddMinutes(60),
				credentials);

			var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

			return token;
		}
	}
}
