using CyberApka.Server.Data.Database;
using CyberApka.Server.Services;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Auth.Commands;

public abstract class RefreshToken
{
    public record Command(RefreshTokenRequest Request) : IRequest<CyberApkaResult<LoginResponse>>;

    public class Handler(CyberDbContext context, TokenService tokenService)
        : IRequestHandler<Command, CyberApkaResult<LoginResponse>>
    {
        private readonly CyberDbContext _context = context;
        private readonly TokenService _tokenService = tokenService;

        public async Task<CyberApkaResult<LoginResponse>> Handle(Command command, CancellationToken ct)
        {
            var request = command.Request;

            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, ct);

            if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return CyberApkaResult<LoginResponse>.Failure("Niepoprawny lub wygasły refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2);

            await _context.SaveChangesAsync(ct);

            return CyberApkaResult<LoginResponse>.Success(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}