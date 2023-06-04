using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;
using System.Reflection;

namespace CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;

public static class CosmosExtensions
{
    internal static CosmosDbSettings _cosmosDbSettings;

    public static void AddCosmosClient(this IServiceCollection services, IConfiguration configuration)
    {
        if (_cosmosDbSettings == null)
        {
            _cosmosDbSettings = new CosmosDbSettings();
            configuration.GetSection("CosmosDbSettings").Bind(_cosmosDbSettings);
            _cosmosDbSettings.PrimaryKey = Environment.GetEnvironmentVariable("CosmosDbSettings__PrimaryKey", EnvironmentVariableTarget.User);
        }

        var cosmosClientOptions = new CosmosClientOptions
        {
            AllowBulkExecution = true
        };

        services.AddSingleton(s => new CosmosClient(_cosmosDbSettings.EndpointUri, _cosmosDbSettings.PrimaryKey, cosmosClientOptions));
    }
    public async static Task CheckAndCreateDatabase(this CosmosDBManager cosmosDBManager)
    {
        if (_cosmosDbSettings != null)
        {
            await cosmosDBManager.CheckConnection();
            await cosmosDBManager.CheckDatabaseExists(_cosmosDbSettings.DatabaseName);
            await cosmosDBManager.CreateDatabase(_cosmosDbSettings.DatabaseName, throughput: 20000);
        }
        else
        {
            throw new InvalidOperationException("CosmosDbSettings is not initialized. Call AddCosmosClient first.");
        }
    }
    public async static Task CreateProjectContainers(this CosmosDBManager cosmosDBManager)
    {
        if (_cosmosDbSettings != null)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(assemblyFolder, "cosmosdb-query-containers-config.json");
            List<ContainerInfo> queryMassiveContainers = JsonUtils.GetContainersFromJsonFile(filePath);
            await cosmosDBManager.CreateContainersList(queryMassiveContainers, _cosmosDbSettings.DatabaseName);
        }
        else
        {
            throw new InvalidOperationException("CosmosDbSettings is not initialized. Call AddCosmosClient first.");
        }
    }
}
