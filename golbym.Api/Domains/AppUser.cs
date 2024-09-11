using Microsoft.AspNetCore.Identity;

namespace golbym.Api.Domains
{
	public class AppUser : IdentityUser
	{
		public DateOnly Dob { get; set; }
		public string Address { get; set; } = string.Empty;
	}
}
