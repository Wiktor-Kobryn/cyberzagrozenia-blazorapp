using CyberApka.Server.Features.Roles.Commands;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Roles;

public class SyncPermissionsEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/api/admin/sync-permissions");
        Permissions("Roles.View");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new SyncPermissions.Command());
        if (result.IsSuccess == false)
        {
            await Send.ErrorsAsync();
            return;
        }

        await Send.OkAsync(result);
     
    }
}
