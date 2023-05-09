using System;
using System.Threading.Tasks;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CqrsShowCase.Tests.Integration.Infrasctructure.Data.AzureCosmosDb;

public class AzureCosmosDbTestFixture : IAsyncLifetime, IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }
    public CosmosClient CosmosClient { get; }
    CosmosDbSettings cosmosDbSettings { get; }
    public string TestDatabaseName { get; } = "TestDatabase";
    public string TestContainerName { get; } = "TestContainer";
    public string TestPartitionKey { get; } = "/id";

    private async Task InitializeTestDatabaseAsync()
    {
        await CosmosClient.CreateDatabaseIfNotExistsAsync(TestDatabaseName);
        await CosmosClient.GetDatabase(TestDatabaseName).CreateContainerIfNotExistsAsync(TestContainerName, TestPartitionKey);
    }

    public async Task CleanUpTestDatabaseAsync()
    {
        await CosmosClient.GetDatabase(TestDatabaseName).DeleteAsync();
    }

    public void Dispose()
    {
        CleanUpTestDatabaseAsync().GetAwaiter().GetResult();
    }

    public async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        CosmosDbSettings cosmosDbSettings = new CosmosDbSettings();
        configuration.Bind("CosmosDbSettings", cosmosDbSettings);
        await InitializeTestDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}
