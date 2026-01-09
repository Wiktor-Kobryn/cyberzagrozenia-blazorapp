using System.ComponentModel.DataAnnotations;
namespace CyberApka.Shared.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Email wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Hasło wymagane.")]
    public string Password { get; set; } = string.Empty;

    public string? CaptchaToken { get; set; }
}
