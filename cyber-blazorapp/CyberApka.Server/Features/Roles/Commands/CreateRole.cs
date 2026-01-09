using CyberApka.Server.Data.Database;
using CyberApka.Server.Data.Entities;
using CyberApka.Server.Services;
using CyberApka.Shared.Requests;
using CyberApka.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CyberApka.Server.Features.Roles.Commands;

public abstract class CreateRole
{
    public record Command(CreateRoleRequest Request) : IRequest<CyberApkaResult<string>>;

    public class Handler(CyberDbContext context, LogService logService) : IRequestHandler<Command, CyberApkaResult<string>>
    {
        private readonly CyberDbContext _context = context;
        private readonly LogService _logService = logService;

        public async Task<CyberApkaResult<string>> Handle(Command command, CancellationToken ct)
        {
            try
            {
                var req = command.Request;
                if (await _context.Roles.AnyAsync(r => r.Name == req.Name, ct))
                    return CyberApkaResult<string>.Failure("Rola o tej nazwie już istnieje.");

                var newRole = new Role
                {
                    Name = req.Name,
                    Description = req.Description
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync(ct);

                await _logService.AddLogAsync("Role Created", $"Created role: {newRole.Name}", null, ct);

                return CyberApkaResult<string>.Success("Rola utworzona pomyślnie.");
            }
            catch (Exception ex)
            {
                return CyberApkaResult<string>.Failure(ex.Message);
            }
        }
    }
}