using System.Threading.Tasks;
using Xunit;
using Microsoft.Azure.Cosmos;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;

namespace CqrsShowCase.Tests.Integration.Infrasctructure.Data.AzureCosmosDb;

public class CosmosCommandEngineIntegrationTests : IClassFixture<AzureCosmosDbTestFixture>
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosCommandEngine _cosmosCommandEngine;
    private readonly AzureCosmosDbTestFixture _fixture;

    public CosmosCommandEngineIntegrationTests(AzureCosmosDbTestFixture fixture)
    {
        _fixture = fixture;
        _cosmosClient = fixture.CosmosClient;

        _cosmosCommandEngine = new CosmosCommandEngine(_cosmosClient);
    }

    [Fact]
    public async Task InsertItemAsync_ShouldInsertNewItem()
    {
        // Arrange
        var testItem = new { id = "1", name = "TestItem", partitionKey = "TestPartitionKey" };

        // Act
        var (insertedItem, requestCharge, errorMessage) = await _cosmosCommandEngine.InsertItemAsync(testItem, _fixture.TestDatabaseName, _fixture.TestContainerName, testItem.partitionKey);

        // Assert
        Assert.Equal(testItem.id, insertedItem.id);
        Assert.Equal(testItem.name, insertedItem.name);
        Assert.Equal(testItem.partitionKey, insertedItem.partitionKey);
        Assert.True(requestCharge > 0);
        Assert.Empty(errorMessage);
    }
}