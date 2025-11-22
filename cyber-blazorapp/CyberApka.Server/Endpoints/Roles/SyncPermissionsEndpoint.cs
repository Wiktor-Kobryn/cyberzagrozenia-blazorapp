using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;
using CyberApka.Shared.Results;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberApka.Server.Endpoints.Roles;

public class SyncPermissionsEndpoint(CyberDbContext context) : EndpointWithoutRequest<CyberApkaResult<string>>
{
    private readonly CyberDbContext _context = context;

    public override void Configure()
    {
        Post("/api/admin/sync-permissions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
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
            await Send.OkAsync(CyberApkaResult<string>.Success("Wszystkie uprawnienia są aktualne. Nic nie dodano."), ct);
            return;
        }

        var adminRole = await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == 2, ct);

        if (adminRole == null)
        {
            await Send.ErrorsAsync(404, ct);
            return;
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
        await Send.OkAsync(CyberApkaResult<string>.Success(msg), ct);
    }
}