using System;
using System.Collections.Generic;
using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CqrsShowCase.Tests.Integration.UserInterface.QueryApi;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:SqlServer"] =
                    "Server=localhost,1433;Database=SocialMediaPostTestApi;User Id=sa;Password={PASSWORD};TrustServerCertificate=True;"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();

            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DatabaseContext>();
            services.RemoveAll<DatabaseContextFactory>();

            var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD", EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD")
                ?? throw new InvalidOperationException("SQLSERVER_PASSWORD environment variable not set.");

            var connectionString =
                $"Server=localhost,1433;Database=SocialMediaPostTestApi;User Id=sa;Password={password};TrustServerCertificate=True;";

            Action<DbContextOptionsBuilder> configureDbContext = o => o
                .UseSqlServer(connectionString);

            services.AddDbContext<DatabaseContext>(configureDbContext);
            services.AddSingleton(new DatabaseContextFactory(configureDbContext));
        });
    }
}
