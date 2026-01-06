using CyberApka.Server.Data.Database;
using CyberApka.Server.Features.Logs.Queries;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using FastEndpoints;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Endpoints.Logs;

public class GetLogsEndpoint(IMediator mediator) : EndpointWithoutRequest<CyberApkaResult<List<LogDto>>>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get("/api/admin/logs");
        Roles("Admin");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetLogs.Query(), ct);

        if (result.IsSuccess == false)
        {
            AddError(result.ErrorMessage ?? "Unknown error");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        await Send.OkAsync(result, ct);
    }
}