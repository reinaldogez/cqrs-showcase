using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Configuration;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb.Managers;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace CqrsShowCase.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    const string ContainerName = "Post";
    private readonly IConfiguration _configuration;
    private readonly CosmosCommandEngine _cosmosCommandEngine;

    public PostRepository(IConfiguration configuration, CosmosCommandEngine cosmosCommandEngine)
    {
        _configuration = configuration;
        _cosmosCommandEngine = cosmosCommandEngine;
    }
    public async Task CreateAsync(PostEntity post)
    {
        await _cosmosCommandEngine.InsertItemAsync(post, CosmosExtensions._cosmosDbSettings.DatabaseName,
            ContainerName, post.PostId.ToString());
    }

    public Task DeleteAsync(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task<PostEntity> GetByIdAsync(Guid postId)
    {
        throw new NotImplementedException();
    }

    public Task<List<PostEntity>> ListAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        throw new NotImplementedException();
    }

    public Task<List<PostEntity>> ListWithCommentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PostEntity post)
    {
        throw new NotImplementedException();
    }
}