using System.ComponentModel.DataAnnotations;

namespace RunGroopWebApp.ViewModels;

public class RegisterViewModel
{
	[Display(Name = "Email Address")]
	[Required(ErrorMessage = "Email Address is required")]
	public string Email { get; set; }
	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }
	[Required(ErrorMessage = "Confirm password is required")]
	[Display(Name = "Confirm Password")]
	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "Password is not match")]
	public string ConfirmPassword { get; set; }
}
