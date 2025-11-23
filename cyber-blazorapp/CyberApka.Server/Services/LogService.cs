using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;

namespace CyberApka.Server.Services;

public class LogService(CyberDbContext context)
{
    private readonly CyberDbContext _context = context;

    public async Task AddLogAsync(string action, string? details = null, int? userId = null, CancellationToken ct = default)
    {
        var log = new Log
        {
            Action = action,
            Details = details,
            UserId = userId,
            TimeStamp = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}