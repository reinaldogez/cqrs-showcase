using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Consumers;
using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using CqrsShowCase.UserInterface.QueryApi;
using CqrsShowCase.Infrastructure.Repositories;
using CqrsShowCase.Query.Domain.Repositories;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR — registers the QueryHandler (which implements IRequestHandler for each query)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CqrsShowCase.Infrastructure.Handlers.FindAllPostsQueryHandler).Assembly));

// SQL Server / EF Core
var connectionString = GetConnectionString(builder.Configuration);
Action<DbContextOptionsBuilder> configureDbContext =
    o => o.UseSqlServer(connectionString);

builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContext));

// Repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Handlers
builder.Services.AddScoped<IEventHandler, CqrsShowCase.Infrastructure.Handlers.EventHandler>();

// Kafka consumer
var consumerConfig = new ConsumerConfig
{
    BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
    GroupId = builder.Configuration["Kafka:GroupId"] ?? "cqrs-query-consumer",
    AutoOffsetReset = AutoOffsetReset.Earliest
};
builder.Services.AddSingleton(consumerConfig);
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

// Ensures that tables exist in SQL Server
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

string GetConnectionString(IConfiguration configuration) =>
    configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("SqlServer connection string not configured.");

// Required so integration tests can reference the Program type via WebApplicationFactory<Program>.
// With top-level statements the compiler generates an implicit internal Program class, and this
// partial declaration makes it public and accessible from the test assembly.
public partial class Program { }
