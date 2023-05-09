using System.Threading.Tasks;
using Xunit;
using Microsoft.Azure.Cosmos;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;
using Xunit.Abstractions;

namespace CqrsShowCase.Tests.Integration.Infrasctructure.Data.AzureCosmosDb;

public class CosmosCommandEngineIntegrationTests : IClassFixture<AzureCosmosDbTestFixture>
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosCommandEngine _cosmosCommandEngine;
    private readonly AzureCosmosDbTestFixture _fixture;

    public CosmosCommandEngineIntegrationTests(
        AzureCosmosDbTestFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        _cosmosClient = fixture.cosmosClient;
        _cosmosCommandEngine = new CosmosCommandEngine(_cosmosClient);
    }

    [Fact]
    public async Task InsertItemAsync_ShouldInsertNewItem()
    {
        // Arrange
        var testItem = new { id = "1", name = "TestItem", partitionKey = "1" };

        // Act
        var (insertedItem, requestCharge, errorMessage) = await _cosmosCommandEngine.InsertItemAsync(testItem, _fixture.cosmosDbSettings.DatabaseName, _fixture.TestContainerName, testItem.partitionKey);

        // Assert
        Assert.Equal(testItem.id, insertedItem.id);
        Assert.Equal(testItem.name, insertedItem.name);
        Assert.Equal(testItem.partitionKey, insertedItem.partitionKey);
        Assert.True(requestCharge > 0);
        Assert.Empty(errorMessage);
    }

    [Fact]
    public async Task DeleteItemAsync_ShouldDeleteItem()
    {
        // Arrange
        var testItem = new { id = "1", name = "TestItem", partitionKey = "1" };
        await _cosmosCommandEngine.InsertItemAsync(testItem, _fixture.cosmosDbSettings.DatabaseName, _fixture.TestContainerName, testItem.partitionKey);

        // Act
        var (success, requestCharge, errorMessage) = await _cosmosCommandEngine.DeleteItemAsync<dynamic>(testItem.id, testItem.partitionKey, _fixture.cosmosDbSettings.DatabaseName, _fixture.TestContainerName);

        // Assert
        Assert.True(success);
        Assert.True(requestCharge > 0);
        Assert.Empty(errorMessage);

        var container = _cosmosClient.GetContainer(_fixture.cosmosDbSettings.DatabaseName, _fixture.TestContainerName);
        var exception = await Assert.ThrowsAsync<CosmosException>(
            async () => await container.ReadItemAsync<dynamic>(testItem.id, new PartitionKey(testItem.partitionKey)));
        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }
}