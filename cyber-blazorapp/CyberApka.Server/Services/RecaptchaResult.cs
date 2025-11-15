using System.Text.Json.Serialization;

namespace CyberApka.Server.Services
{
    public class RecaptchaResult
    {
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}
