// using System;
// using System.Linq;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;

// namespace CqrsShowCase.Infrastructure.Data;
// public static class DatabaseSeeder
// {
//     public static void Seed(IServiceProvider serviceProvider)
//     {
//         using var dbContext = serviceProvider.GetRequiredService<MyDbContext>();

//         // Apply any pending migrations to ensure the database schema is up-to-date
//         dbContext.Database.Migrate();

//         // Seed the database with initial data
//         SeedUsers(dbContext);
//         SeedUserLogs(dbContext);

//         dbContext.SaveChanges();
//     }

//     private static void SeedUsers(MyDbContext dbContext)
//     {
//         // If there are already users in the database, don't seed them again
//         if (dbContext.Users.Any())
//         {
//             return;
//         }

//         // Add some sample users to the database
//         dbContext.Users.Add(new User
//         {
//             Id = 1,
//             Name = "John Doe",
//             Email = "john.doe@example.com",
//             Age = 30
//         });

//         dbContext.Users.Add(new User
//         {
//             Id = 2,
//             Name = "Jane Doe",
//             Email = "jane.doe@example.com",
//             Age = 25
//         });

//         dbContext.SaveChanges();
//     }

//     private static void SeedUserLogs(MyDbContext dbContext)
//     {
//         // If there are already user logs in the database, don't seed them again
//         if (dbContext.UserLogs.Any())
//         {
//             return;
//         }

//         // Add some sample user logs to the database
//         var user1 = dbContext.Users.First();
//         var user2 = dbContext.Users.Last();

//         dbContext.UserLogs.Add(new UserLog
//         {
//             Id = 1,
//             UserId = user1.Id,
//             Action = "Logged in",
//             Timestamp = DateTime.UtcNow
//         });

//         dbContext.UserLogs.Add(new UserLog
//         {
//             Id = 2,
//             UserId = user2.Id,
//             Action = "Registered",
//             Timestamp = DateTime.UtcNow
//         });

//         dbContext.SaveChanges();
//     }
// }