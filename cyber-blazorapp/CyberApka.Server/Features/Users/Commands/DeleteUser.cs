using CyberApka.Server.Data.Database;
using CyberApka.Server.Services;
using CyberApka.Shared.Results;
using MediatR;

namespace CyberApka.Server.Features.Users.Commands;

public abstract class DeleteUser
{
    public record Command(int UserId) : IRequest<CyberApkaResult<string>>;

    public class Handler(CyberDbContext context, LogService logService) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly CyberDbContext _context = context;
        private readonly LogService _logService = logService;

        public async Task<CyberApkaResult<string>> Handle(Command request, CancellationToken ct)
        {
            try
            {
                var user = await _context.Users.FindAsync([request.UserId], ct);
                if (user == null) return CyberApkaResult<string>.Failure("User not found");

                user.IsDeleted = true;

                await _context.SaveChangesAsync(ct);

                await _logService.AddLogAsync("User Deleted (Soft)", $"User {user.Id} deactivated", null, ct);

                return CyberApkaResult<string>.Success("User deleted successfully");
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure(ex.Message);
            }
        }
    }
}