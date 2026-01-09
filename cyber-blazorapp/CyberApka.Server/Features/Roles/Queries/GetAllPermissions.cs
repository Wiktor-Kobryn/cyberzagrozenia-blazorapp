using CyberApka.Server.Data.Database;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Roles.Queries;

public abstract class GetAllPermissions
{
    public record Query() : IRequest<CyberApkaResult<List<PermissionDto>>>;

    public class Handler(CyberDbContext context) : IRequestHandler<Query, CyberApkaResult<List<PermissionDto>>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<List<PermissionDto>>> Handle(Query request, CancellationToken ct)
        {
            var perms = await _context.Permissions
                .AsNoTracking()
                .Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description })
                .ToListAsync(ct);

            return CyberApkaResult<List<PermissionDto>>.Success(perms);
        }
    }
}