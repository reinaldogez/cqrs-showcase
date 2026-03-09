using System;
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
            config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        });

        builder.ConfigureServices((context, services) =>
        {
            services.RemoveAll<IHostedService>();

            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DatabaseContext>();
            services.RemoveAll<DatabaseContextFactory>();

            var baseCs = context.Configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("SqlServer connection string not configured.");

            // Use a dedicated test API database to isolate from the repository integration tests
            var connectionString = baseCs.Replace("SocialMediaPostTest;", "SocialMediaPostTestApi;");

            Action<DbContextOptionsBuilder> configureDbContext = o => o
                .UseSqlServer(connectionString);

            services.AddDbContext<DatabaseContext>(configureDbContext);
            services.AddSingleton(new DatabaseContextFactory(configureDbContext));
        });
    }
}
