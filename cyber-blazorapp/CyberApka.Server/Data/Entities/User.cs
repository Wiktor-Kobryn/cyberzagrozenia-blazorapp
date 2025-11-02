namespace CyberApka.Server.Data.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
