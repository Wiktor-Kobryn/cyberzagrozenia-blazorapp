using System.ComponentModel.DataAnnotations;

namespace CyberApka.Shared.Requests;

public class CreateLogRequest
{
    [Required(ErrorMessage = "Action name is required")]
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
}