using CyberApka.Server.Features.Auth.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CyberApka.Server.Endpoints.Auth;

public class LoginEndpoint(IMediator mediator, IMemoryCache cache) : Endpoint<LoginRequest, CyberApkaResult<LoginResponse>>
{
    private readonly IMediator _mediator = mediator;
    private IMemoryCache _cache = cache;
    private const int MaxAttempts = 3;
    private static readonly TimeSpan LockoutTime = TimeSpan.FromMinutes(15);

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        //var cache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"login-ip-attempts:{ip}";

        // reaching login limit from given IP addr
        if (_cache.TryGetValue<int>(key, out var attempts) && attempts >= MaxAttempts)
        {
            await HttpContext.Response.SendAsync(
                CyberApkaResult<LoginResponse>.Failure($"Zbyt dużo prób logowania. IP zablokowane na {LockoutTime.TotalMinutes} min."),
                429
            );
            return;
        }

        var result = await _mediator.Send(new Login.Command(req));

        if (result.IsSuccess == false)
        {
            // adding failed login attempt for IP
            attempts++;
            _cache.Set(key, attempts, LockoutTime);

            await HttpContext.Response.SendAsync(
                CyberApkaResult<LoginResponse>.Failure(result.ErrorMessage!),
                400
            );
            return;
        }

        // successfull login - clearing attempts
        _cache.Remove(key);
        await Send.OkAsync(result);
    }
}
