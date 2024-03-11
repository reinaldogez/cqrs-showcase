using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace CqrsShowCase.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseContextFactory _contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
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