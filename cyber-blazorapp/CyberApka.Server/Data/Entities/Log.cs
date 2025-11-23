namespace CyberApka.Server.Data.Entities;

public class Log : BaseEntity
{
    public int? UserId { get; set; }
    public User? User { get; set; }
    public required string Action { get; set; }
    public string? Details { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow; //robimy na UTC czy wywalone bo apka w PL?
}
