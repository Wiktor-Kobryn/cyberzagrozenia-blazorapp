using CyberApka.Server.Features.Auth.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Auth;

public class LoginEndpoint(IMediator mediator) : Endpoint<LoginRequest, CyberApkaResult<LoginResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new Login.Command(req));

        if (result.IsSuccess == false)
        {
            AddError(result.ErrorMessage!);
            await Send.ErrorsAsync();
            return;
        }

        await Send.OkAsync(result);
    }
}
