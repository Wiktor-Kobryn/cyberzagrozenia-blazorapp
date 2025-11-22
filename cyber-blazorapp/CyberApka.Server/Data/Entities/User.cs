namespace CyberApka.Server.Data.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] Hash { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
