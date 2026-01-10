using Blazored.LocalStorage;
using CyberApka.Client;
using CyberApka.Client.Providers;
using CyberApka.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<RefreshTokenHandler>();

builder.Services.AddHttpClient("CyberApka.ServerAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7226");
})
    .AddHttpMessageHandler<RefreshTokenHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CyberApka.ServerAPI"));

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>(provider =>
    (CustomAuthStateProvider)provider.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecoveryService>();
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
