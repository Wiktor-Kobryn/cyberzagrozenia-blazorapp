using System.ComponentModel.DataAnnotations;

namespace CyberApka.Shared.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [MinLength(12, ErrorMessage = "Hasło musi mieć conajmniej 12 znaków.")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[\W_]).+$",
        ErrorMessage = "Hasło musi zawierać conajmniej jedną dużą literę i jeden znak specjalny."
    )]
    public string Password { get; set; } = "";

    public string? CaptchaToken { get; set; }
}
