namespace golbym.Api.Models.Dtos
{
	public class LoginModel
	{
		public string Username { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public bool RememberMe { get; set; }
	}
}
