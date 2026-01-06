using CyberApka.Server.Features.Users.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Users;

public class UpdateUserEndpoint(IMediator mediator) : Endpoint<UpdateUserRequest, CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Put("/api/admin/users");
        Roles("Admin");
    }
    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateUser.Command(req), ct);
        await Send.OkAsync(result, ct);
    }
}
