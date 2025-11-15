using FastEndpoints;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace CyberApka.Server.Services
{
    public class RecaptchaService
    {
        private readonly HttpClient _http;
        private readonly string _secret;

        public RecaptchaService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _secret = config["Recaptcha:SecretKey"]
                ?? throw new Exception("Missing recaptcha secret key");
        }

        public async Task<bool> VerifyAsync(string token, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _secret),
                    new KeyValuePair<string, string>("response", token)
                });

            var response = await _http.PostAsync("https://www.google.com/recaptcha/api/siteverify", content, ct );

            var result = await response.Content.ReadFromJsonAsync<RecaptchaResult>(cancellationToken: ct);

            return result?.Success == true;
        }
    }
}