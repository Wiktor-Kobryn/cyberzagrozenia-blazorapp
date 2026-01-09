namespace CyberApka.Shared.Requests;

public class UpdateRolePermissionsRequest
{
    public int RoleId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}