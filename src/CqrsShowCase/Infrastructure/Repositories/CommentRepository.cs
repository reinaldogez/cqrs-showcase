using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace CqrsShowCase.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    const string ContainerName = "CommentContainer";
    private readonly IConfiguration _configuration;
    private readonly CosmosCommandEngine _cosmosCommandEngine;

    public CommentRepository(IConfiguration configuration, CosmosCommandEngine cosmosCommandEngine)
    {
        _configuration = configuration;
        _cosmosCommandEngine = cosmosCommandEngine;
    }

    public Task CreateAsync(CommentEntity comment)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CommentEntity comment)
    {
        throw new NotImplementedException();
    }
}