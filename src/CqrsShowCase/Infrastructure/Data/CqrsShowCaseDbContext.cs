using CqrsShowCase.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CqrsShowCase.Infrastructure.Data;

public class CqrsShowCaseDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<User> Users { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }

    public CqrsShowCaseDbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}