using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace CqrsShowCase.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    const string ContainerName = "Post";
    private readonly IConfiguration _configuration;

    private readonly DatabaseContextFactory _contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Add(post);

        _ = await context.SaveChangesAsync();
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