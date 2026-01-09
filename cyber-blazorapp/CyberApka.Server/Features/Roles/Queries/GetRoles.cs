using CyberApka.Server.Data.Database;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Roles.Queries;

public abstract class GetRoles
{
    public record Query() : IRequest<CyberApkaResult<List<RoleDto>>>;

    public class Handler(CyberDbContext context) : IRequestHandler<Query, CyberApkaResult<List<RoleDto>>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<List<RoleDto>>> Handle(Query request, CancellationToken ct)
        {
            try
            {
                var roles = await _context.Roles
                    .AsNoTracking()
                    .Include(r => r.Permissions)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        Permissions = r.Permissions.Select(p => new PermissionDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description
                        }).ToList()
                    })
                    .ToListAsync(ct);

                return CyberApkaResult<List<RoleDto>>.Success(roles);
            }
            catch (Exception ex)
            {
                return CyberApkaResult<List<RoleDto>>.Failure(ex.Message);
            }
        }
    }
}
