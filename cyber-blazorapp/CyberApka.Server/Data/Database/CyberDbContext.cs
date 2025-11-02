using CyberApka.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberApka.Server.Data.Database;

public class CyberDbContext : DbContext
{
    public CyberDbContext(DbContextOptions<CyberDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Log> Logs{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
