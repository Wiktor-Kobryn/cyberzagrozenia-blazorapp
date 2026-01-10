using CyberApka.Server.Data.Database.Migrations;
using CyberApka.Server.Features.Auth.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Auth;
public class RefreshTokenEndpoint(IMediator mediator) : Endpoint<RefreshTokenRequest, CyberApkaResult<LoginResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshToken.Command(req), ct);

        if (result.IsSuccess)
        {
            await Send.OkAsync(result, ct);
        }
        else
        {
            await Send.OkAsync(result, ct);
        }
    }
}