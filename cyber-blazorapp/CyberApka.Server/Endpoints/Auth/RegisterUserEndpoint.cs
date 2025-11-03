using CyberApka.Server.Features.Auth.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;

namespace CyberApka.Server.Endpoints.Auth;

public class RegisterUserEndpoint : Endpoint<RegisterRequest, CyberApkaResult<RegisterResponse>>
{
    private readonly CreateUser.Handler _handler;

    public RegisterUserEndpoint(CreateUser.Handler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var result = await _handler.Handle(new CreateUser.Command(req), ct);

        if (result.Success == false)
        {
            await Send.ErrorsAsync();
        }

        await Send.OkAsync(result);
    }
}