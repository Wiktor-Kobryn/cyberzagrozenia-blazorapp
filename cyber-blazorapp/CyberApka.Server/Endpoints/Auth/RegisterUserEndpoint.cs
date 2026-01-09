using CyberApka.Server.Features.Auth.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Auth;

public class RegisterUserEndpoint(IMediator mediator) : Endpoint<RegisterRequest, CyberApkaResult<RegisterResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateUser.Command(req));

        if (result.IsSuccess == false)
        {
            await HttpContext.Response.SendAsync(
                CyberApkaResult<LoginResponse>.Failure(result.ErrorMessage!),
                400
            );
            return;
        }

        await Send.OkAsync(result);
    }
}