using System;
using System.Diagnostics;
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
    public CosmosClient cosmosClient { get; private set; }
    public CosmosDbSettings cosmosDbSettings { get; private set; }
    public string TestContainerName { get; } = "TestContainer";
    public string TestPartitionKey { get; } = "/id";
    private async Task InitializeTestDatabaseAsync()
    {
        await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbSettings.DatabaseName);
        await cosmosClient.GetDatabase(cosmosDbSettings.DatabaseName).CreateContainerIfNotExistsAsync(TestContainerName, TestPartitionKey);
    }

    public async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        cosmosDbSettings = new CosmosDbSettings();
        configuration.Bind("CosmosDbSettings", cosmosDbSettings);

        var services = new ServiceCollection();
        CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
        {
            AllowBulkExecution = true
        };
        services.AddSingleton(s => new CosmosClient(cosmosDbSettings.EndpointUri, cosmosDbSettings.PrimaryKey, cosmosClientOptions));
        ServiceProvider = services.BuildServiceProvider();

        cosmosClient = ServiceProvider.GetService<CosmosClient>();
        await InitializeTestDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanUpTestDatabaseAsync();
        ServiceProvider.Dispose();
    }
    public async Task CleanUpTestDatabaseAsync()
    {
        try
        {
            await cosmosClient.GetDatabase(cosmosDbSettings.DatabaseName).DeleteAsync();
        }
        catch (CosmosException ce)
        {
            Exception baseException = ce.GetBaseException();
            Debug.WriteLine($"Exception: Status code {ce.StatusCode}. Exception during deleting database: {cosmosDbSettings.DatabaseName}.\nException details: {ce}");
        }
    }

    public void Dispose()
    {
    }
}
