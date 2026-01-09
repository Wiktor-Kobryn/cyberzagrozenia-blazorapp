using CyberApka.Server.Features.Users.Commands;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;

namespace CyberApka.Server.Endpoints.Users;

public class DeleteUserEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;
    public override void Configure()
    {
        Delete("/api/admin/users/{id}");
        Permissions("Users.Delete");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _mediator.Send(new DeleteUser.Command(id), ct);
        await Send.OkAsync(result, ct);
    }
}