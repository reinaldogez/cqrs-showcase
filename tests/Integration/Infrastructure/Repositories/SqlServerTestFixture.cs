using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using Xunit;

namespace CqrsShowCase.Tests.Integration.Infrastructure.Repositories;

[CollectionDefinition("SqlServer")]
public class SqlServerCollection : ICollectionFixture<SqlServerTestFixture> { }

public class SqlServerTestFixture : IAsyncLifetime, IDisposable
{
    public DatabaseContextFactory ContextFactory { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var template = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("SqlServer connection string not configured in appsettings.json.");

            var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD", EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD")
                ?? throw new InvalidOperationException("SQLSERVER_PASSWORD environment variable not set.");

            var connectionString = template.Replace("{PASSWORD}", password);

            ContextFactory = new DatabaseContextFactory(
                o => o.UseSqlServer(connectionString));

            using var context = ContextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while initializing SqlServerTestFixture: {ex.Message}");
            throw;
        }
    }

    public async Task CleanDatabaseAsync()
    {
        using var context = ContextFactory.CreateDbContext();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [Comment]");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [Post]");
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        using var context = ContextFactory.CreateDbContext();
        context.Database.EnsureDeleted();
    }
}
