using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using System.Net.Http.Json;

namespace CyberApka.Client.Services;
public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CyberApkaResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);
        var result = await response.Content.ReadFromJsonAsync<CyberApkaResult<RegisterResponse>>();
        return result ?? new() { IsSuccess = false, ErrorMessage = "No response from the server" };
    }

    public async Task<CyberApkaResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        var result = await response.Content.ReadFromJsonAsync<CyberApkaResult<LoginResponse>>();
        return result ?? new() { IsSuccess = false, ErrorMessage = "No response from the server" };
    }
}
