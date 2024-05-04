using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CqrsShowCase.Infrastructure.Config;
using CqrsShowCase.Infrastructure.Repositories;
using System.Threading.Tasks;
using CqrsShowCase.Core.Events;
using System.Linq;
using MongoDB.Bson;

namespace CqrsShowCase.Tests.Integration.Infrastructure.Repositories;

public class EventStoreRepositoryIntegrationTests : IDisposable
{
    private readonly IOptions<MongoDbConfig> _config;
    private readonly MongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly EventStoreRepository _repository;

    public EventStoreRepositoryIntegrationTests()
    {
        // Setup Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        var mongoDbConfig = new MongoDbConfig();
        configuration.GetSection("MongoDbConfig").Bind(mongoDbConfig);
        _config = Options.Create(mongoDbConfig);

        // Create MongoClient and database
        _mongoClient = new MongoClient(_config.Value.ConnectionString);
        _database = _mongoClient.GetDatabase(_config.Value.Database);
        _database.DropCollection(_config.Value.Collection); // Ensure collection is clean before tests

        // Instantiate the repository
        _repository = new EventStoreRepository(_config);
    }

    [Fact]
    public async Task DatabaseConnection_IsSuccessful()
    {
        try
        {
            // Act
            var databases = await _mongoClient.ListDatabaseNames().ToListAsync();

            // Assert
            Assert.True(databases.Any(), "Connection failed: No databases found.");
        }
        catch (MongoConnectionException ex)
        {
            Assert.True(false, $"Error testing MongoDB connection: {ex.Message}");
        }
    }

    [Fact]
    public async Task SaveAsync_SavesEventSuccessfully()
    {
        // Arrange
        var eventModel = new EventModel { Id = ObjectId.GenerateNewId().ToString(), AggregateIdentifier = Guid.NewGuid() };

        // Act
        await _repository.SaveAsync(eventModel);
        var collection = _database.GetCollection<EventModel>(_config.Value.Collection);
        var result = await collection.Find(x => x.Id == eventModel.Id).FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventModel.Id, result.Id);
    }

    [Fact]
    public async Task FindByAggregateId_ReturnsCorrectEvents()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var eventModel = new EventModel { Id = ObjectId.GenerateNewId().ToString(), AggregateIdentifier = aggregateId };

        // Act
        var collection = _database.GetCollection<EventModel>(_config.Value.Collection);
        await collection.InsertOneAsync(eventModel);
        var results = await _repository.FindByAggregateId(aggregateId);

        // Assert
        Assert.Single(results);
        Assert.Equal(eventModel.Id, results[0].Id);
    }

    public void Dispose()
    {
        // Cleanup database
        _mongoClient.DropDatabase(_config.Value.Database);
    }
}
