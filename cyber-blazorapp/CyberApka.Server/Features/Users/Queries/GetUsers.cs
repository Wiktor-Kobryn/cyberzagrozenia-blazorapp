using CyberApka.Server.Data.Database;
using CyberApka.Shared.DTOs;
using CyberApka.Shared.Responses;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Users.Queries;

public abstract class GetUsers
{
    public record Query() : IRequest<CyberApkaResult<GetUsersResponse>>;

    public class Handler(CyberDbContext context) : IRequestHandler<Query, CyberApkaResult<GetUsersResponse>>
    {
        private readonly CyberDbContext _context = context;

        public async Task<CyberApkaResult<GetUsersResponse>> Handle(Query request, CancellationToken ct)
        {
            try
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Role)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        RoleId = u.RoleId,
                        RoleName = u.Role.Name
                    })
                    .ToListAsync(ct);

                var roles = await _context.Roles
                    .AsNoTracking()
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Name = r.Name
                    })
                    .ToListAsync(ct);

                var responseData = new GetUsersResponse
                {
                    Users = users,
                    Roles = roles
                };

                return CyberApkaResult<GetUsersResponse>.Success(responseData);
            }
            catch (Exception ex)
            {
                return CyberApkaResult<GetUsersResponse>.Failure(ex.Message);
            }
        }
    }
}