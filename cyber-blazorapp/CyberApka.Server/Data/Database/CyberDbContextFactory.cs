using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CyberApka.Server.Data.Database;

public class CyberDbContextFactory : IDesignTimeDbContextFactory<CyberDbContext>
{
    public CyberDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CyberDbContext>();

        optionsBuilder.UseSqlServer(
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CyberApkaDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        return new CyberDbContext(optionsBuilder.Options);
    }
}