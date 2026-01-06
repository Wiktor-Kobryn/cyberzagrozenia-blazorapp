using CyberApka.Server.Data.Database;
using CyberApka.Server.Services;
using CyberApka.Shared.Results;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CyberApka.Server.Endpoints.Auth;

public class LogoutEndpoint(CyberDbContext context, LogService logService) : EndpointWithoutRequest<CyberApkaResult<string>>
{
    private readonly CyberDbContext _context = context;
    private readonly LogService _logService = logService;

    public override void Configure()
    {
        Post("/api/auth/logout");
        Claims("sub");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

            if (int.TryParse(userIdString, out int userId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user != null)
                {
                    string userEmail = user.Email;

                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;

                    await _context.SaveChangesAsync(ct);

                    await _logService.AddLogAsync("Logout success", userEmail, userId, ct);
                }
                else
                {
                    await _logService.AddLogAsync("Logout - User not found in DB", $"ID from claim: {userId}", userId, ct);
                }
            }
            else
            {
                await _logService.AddLogAsync("Logout - Invalid Claim Format", userIdString ?? "null", null, ct);
            }

            await Send.OkAsync(CyberApkaResult<string>.Success("Logged out successfully"), ct);
        }
        catch (Exception ex)
        {
            var userIdString = User.FindFirst("sub")?.Value;
            int.TryParse(userIdString, out int userId);

            await _logService.AddLogAsync("Logout - Error", ex.Message, userId == 0 ? null : userId, ct);

            await Send.OkAsync(CyberApkaResult<string>.Failure("Logout failed due to server error"), ct);
        }

      
    }
}