using CqrsShowCase.Query.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
}