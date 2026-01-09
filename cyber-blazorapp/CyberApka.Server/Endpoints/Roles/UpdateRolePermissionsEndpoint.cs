using CyberApka.Server.Features.Roles.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Roles;

public class UpdateRolePermissionsEndpoint(IMediator mediator) : Endpoint<UpdateRolePermissionsRequest, CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Put("/api/admin/roles/permissions");
        Permissions("Roles.Manage");
    }
    public override async Task HandleAsync(UpdateRolePermissionsRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateRolePermissions.Command(req), ct);
        await Send.OkAsync(result, ct);
    }
}