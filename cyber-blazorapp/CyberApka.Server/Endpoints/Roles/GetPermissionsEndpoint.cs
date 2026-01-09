using CyberApka.Server.Features.Roles.Queries;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Roles;

public class GetPermissionsEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<List<PermissionDto>>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Get("/api/admin/permissions");
        Permissions("Roles.View");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllPermissions.Query(), ct);
        await Send.OkAsync(result, ct);
    }
}
