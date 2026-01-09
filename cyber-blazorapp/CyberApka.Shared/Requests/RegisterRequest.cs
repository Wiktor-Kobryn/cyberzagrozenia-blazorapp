using System.ComponentModel.DataAnnotations;

namespace CyberApka.Shared.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters long.")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter and one special character."
    )]
    public string Password { get; set; } = "";

    public string? CaptchaToken { get; set; }
}
