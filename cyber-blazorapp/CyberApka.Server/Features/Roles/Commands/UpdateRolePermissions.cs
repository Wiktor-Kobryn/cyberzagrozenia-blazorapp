using CyberApka.Server.Data.Database;
using CyberApka.Server.Services;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Roles.Commands;

public abstract class UpdateRolePermissions
{
    public record Command(UpdateRolePermissionsRequest Request) : IRequest<CyberApkaResult<string>>;

    public class Handler(CyberDbContext context, LogService logService) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly CyberDbContext _context = context;
        private readonly LogService _logService = logService;

        public async Task<CyberApkaResult<string>> Handle(Command command, CancellationToken ct)
        {
            try
            {
                var req = command.Request;

                var role = await _context.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefaultAsync(r => r.Id == req.RoleId, ct);

                if (role == null) return CyberApkaResult<string>.Failure("Nie znaleziono roli.");

                var permissionsToAdd = await _context.Permissions
                    .Where(p => req.PermissionIds.Contains(p.Id))
                    .ToListAsync(ct);

                role.Permissions.Clear();
                foreach (var perm in permissionsToAdd)
                {
                    role.Permissions.Add(perm);
                }

                await _context.SaveChangesAsync(ct);
                await _logService.AddLogAsync("Role Permissions Updated", $"Role ID: {role.Id}, Perms count: {role.Permissions.Count}", null, ct);

                return CyberApkaResult<string>.Success("Uprawnienia zaktualizowane pomyślnie.");
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure(ex.Message);
            }
        }
    }
}