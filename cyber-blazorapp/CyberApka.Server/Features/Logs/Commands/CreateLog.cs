using CyberApka.Server.Services;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using MediatR;

namespace CyberApka.Server.Features.Logs.Commands;

public abstract class CreateLog
{
    public record Command(CreateLogRequest Request, int? UserId) : IRequest<CyberApkaResult<string>>;

    public class Handler(LogService logService) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly LogService _logService = logService;

        public async Task<CyberApkaResult<string>> Handle(Command command, CancellationToken ct)
        {
            try
            {
                await _logService.AddLogAsync(
                    action: command.Request.Action,
                    details: command.Request.Details,
                    userId: command.UserId,
                    ct: ct
                );

                return CyberApkaResult<string>.Success("Log added");
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure($"Logging failed: {ex.Message}");
            }
        }
    }
}