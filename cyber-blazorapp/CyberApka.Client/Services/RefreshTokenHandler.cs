using Blazored.LocalStorage;
using CyberApka.Client.Providers;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CyberApka.Client.Services;
public class RefreshTokenHandler(ILocalStorageService localStorage, NavigationManager navManager, IServiceProvider serviceProvider) : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly NavigationManager _navManager = navManager;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

                if (string.IsNullOrEmpty(refreshToken))
                {
                    await ForceLogout();
                    return response;
                }

                var refreshClient = new HttpClient { BaseAddress = new Uri("https://localhost:7226") };

                var refreshResult = await refreshClient.PostAsJsonAsync("api/auth/refresh", new { RefreshToken = refreshToken });

                if (refreshResult.IsSuccessStatusCode)
                {
                    var result = await refreshResult.Content.ReadFromJsonAsync<CyberApkaResult<LoginResponse>>();

                    if (result != null && result.IsSuccess)
                    {
                        await _localStorage.SetItemAsync("authToken", result.Data.AccessToken);
                        await _localStorage.SetItemAsync("refreshToken", result.Data.RefreshToken);

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                        return await base.SendAsync(request, cancellationToken);
                    }
                }
            }
            catch (Exception)
            {
               
            }

            await ForceLogout();
        }

        return response;
    }

    private async Task ForceLogout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");

        var authStateProvider = _serviceProvider.GetRequiredService<AuthenticationStateProvider>();

      
        if (authStateProvider is CustomAuthStateProvider customProvider)
        {
            await customProvider.GetAuthenticationStateAsync();
        }

        _navManager.NavigateTo("/login");
    }
}