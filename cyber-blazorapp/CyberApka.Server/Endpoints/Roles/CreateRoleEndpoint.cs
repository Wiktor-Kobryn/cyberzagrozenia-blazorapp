using CyberApka.Server.Features.Roles.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Roles;

public class CreateRoleEndpoint(IMediator mediator) : Endpoint<CreateRoleRequest, CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Post("/api/admin/roles");
        Permissions("Roles.Manage");
    }
    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRole.Command(req), ct);
        await Send.OkAsync(result, ct);
    }
}
