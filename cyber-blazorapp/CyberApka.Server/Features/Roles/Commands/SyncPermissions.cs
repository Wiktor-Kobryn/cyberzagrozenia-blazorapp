using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberApka.Server.Features.Roles.Commands;

public abstract class SyncPermissions
{
    public record Command() : IRequest<CyberApkaResult<string>>;

    public class Handler(CyberDbContext context) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<string>> Handle(Command command, CancellationToken ct)
        {
            try
            {
                var permissionsInCode = typeof(Permissions)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .Select(fi => (string)fi.GetValue(null)!)
                    .ToList();

                var existingPermissions = await _context.Permissions
                    .Select(p => p.Name)
                    .ToListAsync(ct);

                var missingPermissions = permissionsInCode.Except(existingPermissions).ToList();

                if (!missingPermissions.Any())
                {
                    return CyberApkaResult<string>.Success("Brak uprawnień do zaktualizowania.");
                }

                var adminRole = await _context.Roles
                    .Include(r => r.Permissions)
                    .FirstOrDefaultAsync(r => r.Id == 2, ct);

                if (adminRole == null)
                {
                    return CyberApkaResult<string>.Failure("Error while sync permissions");
                }

                var newEntities = new List<Permission>();

                foreach (var permissionName in missingPermissions)
                {
                    var newPerm = new Permission
                    {
                        Name = permissionName,
                        Description = $"Auto-generated: {permissionName}"
                    };

                    newEntities.Add(newPerm);

                    _context.Permissions.Add(newPerm);

                    adminRole.Permissions.Add(newPerm);
                }

                await _context.SaveChangesAsync(ct);
                var msg = $"Dodano {missingPermissions.Count} nowych uprawnień: {string.Join(", ", missingPermissions)}";
                return CyberApkaResult<string>.Success(msg);
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure(ex.Message);
            }
        }
    }
}
