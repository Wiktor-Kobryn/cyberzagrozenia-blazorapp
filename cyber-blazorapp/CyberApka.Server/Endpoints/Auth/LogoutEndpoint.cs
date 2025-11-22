using CyberApka.Server.Data.Database;
using CyberApka.Shared.Results;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CyberApka.Server.Endpoints.Auth;

public class LogoutEndpoint(CyberDbContext context) : EndpointWithoutRequest<CyberApkaResult<string>>
{
    private readonly CyberDbContext _context = context;

    public override void Configure()
    {
        Post("/api/auth/logout");
        // Ważne: Tylko zalogowany użytkownik może się wylogować
        // To wymaga skonfigurowanego JWT Bearer Auth w Program.cs (Server)!
        // Zakładam, że wiesz jak dodać app.UseAuthentication() i AddJwtBearer
        Claims("sub"); // Wymagamy claima 'sub' (ID użytkownika)
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

        if (int.TryParse(userIdString, out int userId))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await _context.SaveChangesAsync(ct);
            }
        }

        await Send.OkAsync(CyberApkaResult<string>.Success("Logged out successfully"), ct);
    }
}