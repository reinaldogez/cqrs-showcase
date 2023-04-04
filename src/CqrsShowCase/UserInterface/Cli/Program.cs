using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CqrsShowCase.Infrastructure.Data;

var services = new ServiceCollection();
services.AddDbContext<CqrsShowCaseDbContext>();

var serviceProvider = services.BuildServiceProvider();

try
{
    var dbContext = serviceProvider.GetRequiredService<CqrsShowCaseDbContext>();
    dbContext.Database.EnsureCreated();
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while creating the database: " + ex.Message);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
