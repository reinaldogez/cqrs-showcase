using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CqrsShowCase.Core.Infrascructure;
using CqrsShowCase.Infrastructure.Stores;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using Confluent.Kafka;
using CqrsShowCase.Infrastructure.Messaging.Config;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;
using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Consumers;
using CqrsShowCase.Core.Producers;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Query.Domain.Repositories;
using CqrsShowCase.Infrastructure.Repositories;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddScoped<IEventStore, EventStore>();
services.AddScoped<IEventHandler, CqrsShowCase.Infrastructure.Handlers.EventHandler>();

services.AddCosmosClient(configuration);
services.AddScoped<CosmosDBManager>();
services.AddScoped<CosmosCommandEngine>();

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
services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<ICommentRepository, CommentRepository>();

var serviceProvider = services.BuildServiceProvider();

var cosmosDBManager = serviceProvider.GetRequiredService<CosmosDBManager>();
await cosmosDBManager.CheckAndCreateDatabase();
await cosmosDBManager.CreateProjectContainers();

using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
CancellationTokenSource cts = new CancellationTokenSource();
Task consumerTask = null;
try
{
    var eventConsumer = serviceProvider.GetRequiredService<IEventConsumer>();

    string topicName = Environment.GetEnvironmentVariable("KAFKA_TOPIC", EnvironmentVariableTarget.User);
    consumerTask = Task.Run(() => eventConsumer.Consume(topicName, cts.Token));
    var eventStore = serviceProvider.GetRequiredService<IEventStore>();
    CreateAndPostEventOnEventStore(eventStore);
    await Task.Delay(10000);
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

void CreateAndPostEventOnEventStore(IEventStore eventStore)
{
    PostCreatedEvent eventData = new(); // Replace with your actual derived event class
    eventData.Id = Guid.NewGuid();
    IEnumerable<BaseEvent> events = new List<BaseEvent>() { eventData };
    eventStore.SaveEventsAsync(Guid.NewGuid(), events, 0);
}