using System.ComponentModel.DataAnnotations;
namespace CyberApka.Shared.Requests;

public class RecoveryEmailRequest
{
    [Required(ErrorMessage = "Email required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    public string? CaptchaToken { get; set; }
}
