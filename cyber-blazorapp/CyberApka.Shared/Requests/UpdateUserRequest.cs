namespace CyberApka.Shared.Requests;

public class UpdateUserRequest
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
}
