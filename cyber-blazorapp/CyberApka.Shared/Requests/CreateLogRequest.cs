using System.ComponentModel.DataAnnotations;

namespace CyberApka.Shared.Requests;

public class CreateLogRequest
{
    [Required(ErrorMessage = "Nazwa loga jest wymagana")]
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
}
