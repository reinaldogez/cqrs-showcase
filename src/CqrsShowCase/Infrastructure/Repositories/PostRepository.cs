using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CqrsShowCase.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
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

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Posts.Update(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await context.Posts.FindAsync(postId);
        if (post == null) return;
        context.Posts.Remove(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.FindAsync(postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking().ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }
}
