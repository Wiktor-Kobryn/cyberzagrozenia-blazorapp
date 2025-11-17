using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using System.Net.Http.Json;

namespace CyberApka.Client.Services;
public class RecoveryService
{
    private readonly HttpClient _http;

    public RecoveryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CyberApkaResult<RecoveryEmailResponse>> SendRecoveryEmailAsync(RecoveryEmailRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/recovery", request);
        var result = await response.Content.ReadFromJsonAsync<CyberApkaResult<RecoveryEmailResponse>>();
        return result ?? new() { IsSuccess = false, ErrorMessage = "No response from the server" };
    }
}
