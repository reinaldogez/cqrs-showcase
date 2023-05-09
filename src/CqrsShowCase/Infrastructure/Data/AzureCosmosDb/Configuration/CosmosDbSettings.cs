namespace CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;

public class CosmosDbSettings
{
    public string EndpointUri { get; set; }
    public string PrimaryKey { get; set; }
    public string DatabaseName { get; set; }
}