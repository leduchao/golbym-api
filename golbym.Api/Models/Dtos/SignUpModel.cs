﻿using System.ComponentModel.DataAnnotations;

namespace golbym.Api.Models.Dtos
{
	public class SignUpModel
	{
		[Required]
		public string Username { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		//[Required]
		//public string ConfirmPassword { get; set; } = string.Empty;
	}
}
