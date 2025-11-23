using CyberApka.Server.Features.Logs.Commands;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;
using System.Security.Claims;

namespace CyberApka.Server.Endpoints.Logs;

public class CreateLogEndpoint(IMediator mediator) : Endpoint<CreateLogRequest, CyberApkaResult<string>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Post("/api/logs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateLogRequest req, CancellationToken ct)
    {
        int? userId = null;
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

        if (int.TryParse(userIdString, out int parsedId))
        {
            userId = parsedId;
        }

        var result = await _mediator.Send(new CreateLog.Command(req, userId), ct);

        if (result.IsSuccess == false)
        {
            await Send.ErrorsAsync();
            return;
        }

        await Send.OkAsync(result);
    }
}