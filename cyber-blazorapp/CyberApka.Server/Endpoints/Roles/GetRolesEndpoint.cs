using CyberApka.Server.Features.Roles.Queries;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Roles;

public class GetRolesEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<List<RoleDto>>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Get("/api/admin/roles");
        Permissions("Roles.View");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoles.Query(), ct);
        await Send.OkAsync(result, ct);
    }
}
