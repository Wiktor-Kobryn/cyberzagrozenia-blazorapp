using CyberApka.Server.Features.Users.Queries;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Users;

public class GetUsersEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<GetUsersResponse>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/api/admin/users");
        Roles("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUsers.Query(), ct);

        if (!result.IsSuccess)
            await Send.ErrorsAsync(cancellation: ct);
        else
            await Send.OkAsync(result, ct);
    }
}