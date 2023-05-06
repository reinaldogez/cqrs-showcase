using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CqrsShowCase.Infrastructure.Data;


var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);

var serviceProvider = services.BuildServiceProvider();

try
{
    // var dbContext = serviceProvider.GetRequiredService<CqrsShowCaseDbContext>();
    // dbContext.Database.EnsureCreated();
    // DatabaseSeeder.Seed(serviceProvider);
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while creating the database: " + ex.Message);
}
