using CyberApka.Server.Data.Database;
using CyberApka.Server.Services;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using MediatR;

namespace CyberApka.Server.Features.Users.Commands;

public abstract class UpdateUser
{
    public record Command(UpdateUserRequest Dto) : IRequest<CyberApkaResult<string>>;

    public class Handler(CyberDbContext context, LogService logService) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly CyberDbContext _context = context;
        private readonly LogService _logService = logService;

        public async Task<CyberApkaResult<string>> Handle(Command request, CancellationToken ct)
        {
            try
            {
                var dto = request.Dto;
                var user = await _context.Users.FindAsync([dto.Id], ct);

                if (user == null) return CyberApkaResult<string>.Failure("User not found");

                await _logService.AddLogAsync("User Update", $"Changed data for ID: {user.Id}. Role: {user.RoleId}->{dto.RoleId}", null, ct);

                user.Username = dto.Username;
                user.Email = dto.Email;
                user.RoleId = dto.RoleId;

                await _context.SaveChangesAsync(ct);
                return CyberApkaResult<string>.Success("User updated successfully");
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure(ex.Message);
            }
        }
    }
}
