using CqrsShowCase.Query.Domain.Entities;
using Newtonsoft.Json;

namespace CqrsShowCase.Infrastructure.Data.AzureCosmosDb;

public class CosmosPostEntity : PostEntity
{
    [JsonProperty("id")]
    public string CosmosId => PostId.ToString();
}