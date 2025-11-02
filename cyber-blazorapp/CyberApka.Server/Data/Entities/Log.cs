namespace CyberApka.Server.Data.Entities;

public class Log : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public string Action { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now; //robimy na UTC czy wywalone bo apka w PL?
}
