using Blazored.LocalStorage;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CyberApka.Client.Services;
public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
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

        if (result is not null && result.IsSuccess && result.Data is not null)
        {
            await _localStorage.SetItemAsync("authToken", result.Data.AccessToken);
            await _localStorage.SetItemAsync("refreshToken", result.Data.RefreshToken);

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
        }

        return result ?? new() { IsSuccess = false, ErrorMessage = "No response from the server" };
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _http.PostAsync("api/auth/logout", null);
        }
        catch {}

        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");

        _http.DefaultRequestHeaders.Authorization = null;

        await _authStateProvider.GetAuthenticationStateAsync();
    }
}
