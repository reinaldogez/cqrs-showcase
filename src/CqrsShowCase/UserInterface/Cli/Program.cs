using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CqrsShowCase.Infrastructure.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<CqrsShowCaseDbContext>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<CqrsShowCaseDbContext>();
    context.Database.EnsureCreated();
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred while creating the database: " + ex.Message);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
