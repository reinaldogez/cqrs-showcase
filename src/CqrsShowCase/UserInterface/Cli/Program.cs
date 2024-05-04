using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CqrsShowCase.Core.Infrascructure;
using CqrsShowCase.Infrastructure.Stores;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using Confluent.Kafka;
using CqrsShowCase.Infrastructure.Messaging.Config;
using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Consumers;
using CqrsShowCase.Core.Producers;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Query.Domain.Repositories;
using CqrsShowCase.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Infrastructure.Config;
using CqrsShowCase.Core.Domain;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = BuildConfiguration();
var services = new ServiceCollection();
ConfigureServices(services, builder);

var serviceProvider = services.BuildServiceProvider();

//StartKafka();

// Check for command-line arguments to determine the action
if (args.Length > 0)
{
    switch (args[0].ToLower())
    {
        case "testsql":
            await TestDatabaseConnection(serviceProvider);
            break;
        case "testmongodb":
            await TestMongoDbConnection(serviceProvider);
            break;
        default:
            Console.WriteLine($"Unknown command: {args[0]}");
            break;
    }
}
else
{
    Console.WriteLine("No command provided. Available commands: 'testsql', 'testmongodb'");
}

IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();
}

async void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IConfiguration>(configuration);

    services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));
    services.AddScoped<IEventStoreRepository, EventStoreRepository>();
    services.AddScoped<IEventStore, EventStore>();

    var connectionString = GetDatabaseConnectionString(configuration);
    Console.WriteLine($"SQL Server Connection String: {connectionString}");
    Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseLazyLoadingProxies().UseSqlServer(connectionString));
    services.AddDbContext<DatabaseContext>(configureDbContext);
    services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

    services.AddScoped<IPostRepository, PostRepository>();
    services.AddScoped<ICommentRepository, CommentRepository>();

    services.AddScoped<IEventStore, EventStore>();
    services.AddScoped<IEventHandler, CqrsShowCase.Infrastructure.Handlers.EventHandler>();

    ProducerConfig configProducer = await ConfigFiles.LoadConfig<ProducerConfig>(@"..\..\Infrastructure\Messaging\Config\kafkaproducer.config");
    ConsumerConfig configConsumer = await ConfigFiles.LoadConfig<ConsumerConfig>(@"..\..\Infrastructure\Messaging\Config\kafkaconsumer.config");

    if (configProducer == null || configConsumer == null)
    {
        throw new Exception($"An error occurred while reading the Kafka configuration files.");
    }

    services.AddSingleton<ProducerConfig>(configProducer);
    services.AddSingleton<ConsumerConfig>(configConsumer);
    services.AddScoped<IEventProducer, EventProducer>();
    services.AddScoped<IEventConsumer, EventConsumer>();

}

string GetDatabaseConnectionString(IConfiguration configuration)
{
    var connectionStringTemplate = configuration.GetConnectionString("SqlServer");
    var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD", EnvironmentVariableTarget.User);
    return connectionStringTemplate.Replace("{PASSWORD}", password);
}

async Task TestDatabaseConnection(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        Console.WriteLine(canConnect ? "Database connection successful." : "Failed to connect to the database.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error testing database connection: {ex.Message}");
    }
}

async Task TestMongoDbConnection(IServiceProvider serviceProvider)
{
    var mongoDbConfig = serviceProvider.GetRequiredService<IOptions<MongoDbConfig>>().Value;
    var connectionString = mongoDbConfig.ConnectionString;

    var client = new MongoClient(connectionString);

    try
    {
        var databases = await client.ListDatabaseNames().ToListAsync();
        if (databases.Any())
        {
            Console.WriteLine("MongoDB connection successful. Databases:");
            foreach (var db in databases)
            {
                Console.WriteLine($" - {db}");
            }
        }
        else
        {
            Console.WriteLine("MongoDB connection successful, but no databases found.");
        }
    }
    catch (MongoConnectionException ex)
    {
        Console.WriteLine($"Error testing MongoDB connection: {ex.Message}");
    }
}

async void StartKafka()
{
    CancellationTokenSource cts = new CancellationTokenSource();

    Console.CancelKeyPress += (s, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
    };
    Task consumerTask = null;
    try
    {
        var eventConsumer = serviceProvider.GetRequiredService<IEventConsumer>();

        string topicName = Environment.GetEnvironmentVariable("KAFKA_TOPIC", EnvironmentVariableTarget.User);
        consumerTask = Task.Run(() => eventConsumer.Consume(topicName, cts.Token));
        var eventStore = serviceProvider.GetRequiredService<IEventStore>();
        CreateAndPostEventOnEventStore(eventStore);
        await consumerTask;
    }
    catch (TaskCanceledException ex)
    {
        Console.WriteLine($"Exception on EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully: {Environment.NewLine}" +
            $"The operation timed out {ex.Message}");
        throw new Exception("The operation timed out", ex);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception on EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully{ex.Message}");
    }
    finally
    {
        //the broker needs to ensure that there are no active consumers or producers using
        //the topic before it can safely remove it.
        cts.Cancel();

        try
        {
            // Wait for the consumer to finish
            await consumerTask;
        }
        catch (OperationCanceledException)
        {
            // Ignore the exception since we're cancelling the consumer deliberately
        }

    }
}

void CreateAndPostEventOnEventStore(IEventStore eventStore)
{
    PostCreatedEvent eventData = new(); // Replace with your actual derived event class
    eventData.Id = Guid.NewGuid();
    IEnumerable<BaseEvent> events = new List<BaseEvent>() { eventData };
    eventStore.SaveEventsAsync(Guid.NewGuid(), events, 0);
}