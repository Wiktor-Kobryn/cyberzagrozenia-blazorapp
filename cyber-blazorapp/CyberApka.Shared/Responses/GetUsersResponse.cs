using CyberApka.Shared.DTOs;

namespace CyberApka.Shared.Responses;

public class GetUsersResponse
{
    public List<UserDto> Users { get; set; } = new();
    public List<RoleDto> Roles { get; set; } = new();
}