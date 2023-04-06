using System;
using System.Linq;
using Bogus;
using CqrsShowCase.Command.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsShowCase.Infrastructure.Data;
public static class DatabaseSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var dbContext = serviceProvider.GetRequiredService<CqrsShowCaseDbContext>();

        // Apply any pending migrations to ensure the database schema is up-to-date
        dbContext.Database.Migrate();

        // Seed the database with initial data
        SeedUsers(dbContext);

        dbContext.SaveChanges();
    }

    private static void SeedUsers(CqrsShowCaseDbContext dbContext)
    {
        // If there are already users in the database, don't seed them again
        if (dbContext.Users.Any())
        {
            return;
        }

        try
        {
            {
                var users = new Faker<User>()
                    .RuleFor(u => u.Name, f => f.Name.FullName())
                    .RuleFor(u => u.Email, f => f.Internet.Email())
                    .RuleFor(u => u.Age, f => f.Random.Number(18, 65))
                    .RuleFor(u => u.Logs, f => new Faker<UserLog>()
                        .RuleFor(l => l.Action, f => f.Random.Word())
                        .RuleFor(l => l.Timestamp, f => f.Date.Past())
                        .Generate(f.Random.Number(1, 10)))
                    .Generate(10000);


                dbContext.Users.AddRange(users);
                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
        }
    }

}