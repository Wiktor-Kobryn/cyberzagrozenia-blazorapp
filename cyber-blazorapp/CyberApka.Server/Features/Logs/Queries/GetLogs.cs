using CyberApka.Server.Data.Database;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Logs.Queries;

public abstract class GetLogs
{
    public record Query() : IRequest<CyberApkaResult<List<LogDto>>>;

    public class Handler(CyberDbContext context) : IRequestHandler<Query, CyberApkaResult<List<LogDto>>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<List<LogDto>>> Handle(Query request, CancellationToken ct)
        {
            try
            {
                // Logika pobierania logów
                var logs = await _context.Logs
                    .AsNoTracking()
                    .Include(l => l.User)
                    .OrderByDescending(l => l.TimeStamp)
                    .Take(100)
                    .Select(l => new LogDto(
                        l.Id,
                        l.Action,
                        l.Details,
                        l.User != null ? l.User.Email : "Anonymous",
                        l.TimeStamp
                    ))
                    .ToListAsync(ct);

                return CyberApkaResult<List<LogDto>>.Success(logs);
            }
            catch (Exception ex)
            {
                return CyberApkaResult<List<LogDto>>.Failure($"Failed to fetch logs: {ex.Message}");
            }
        }
    }
}