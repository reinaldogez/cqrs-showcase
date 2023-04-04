using CqrsShowCase.Domain;
using Microsoft.EntityFrameworkCore;

namespace CqrsShowCase.Infrastructure.Data;

public class CqrsShowCaseDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlServer("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;");
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CqrsShowCase;User Id=sa;Password=my(!)Password;TrustServerCertificate=True;");
    }
}