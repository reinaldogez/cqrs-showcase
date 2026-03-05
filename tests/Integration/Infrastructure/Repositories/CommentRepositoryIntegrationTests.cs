using System;
using System.Threading.Tasks;
using CqrsShowCase.Infrastructure.Repositories;
using CqrsShowCase.Query.Domain.Entities;
using Xunit;

namespace CqrsShowCase.Tests.Integration.Infrastructure.Repositories;

[Collection("SqlServer")]
public class CommentRepositoryIntegrationTests
{
    private readonly SqlServerTestFixture _fixture;
    private readonly CommentRepository _commentRepository;
    private readonly PostRepository _postRepository;

    public CommentRepositoryIntegrationTests(SqlServerTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.CleanDatabaseAsync().GetAwaiter().GetResult();
        _commentRepository = new CommentRepository(_fixture.ContextFactory);
        _postRepository = new PostRepository(_fixture.ContextFactory);
    }

    private async Task<PostEntity> CreateAndInsertPostAsync()
    {
        var post = new PostEntity
        {
            PostId = Guid.NewGuid(),
            Author = "TestAuthor",
            DatePosted = DateTime.UtcNow,
            Message = "Parent post",
            Likes = 0
        };
        await _postRepository.CreateAsync(post);
        return post;
    }

    private static CommentEntity CreateComment(Guid postId, string username = "user1", string comment = "A comment")
    {
        return new CommentEntity
        {
            CommentId = Guid.NewGuid(),
            Username = username,
            CommentDate = DateTime.UtcNow,
            Comment = comment,
            Edited = false,
            PostId = postId
        };
    }

    [Fact]
    public async Task CreateAsync_PersistsCommentToDatabase()
    {
        var post = await CreateAndInsertPostAsync();
        var comment = CreateComment(post.PostId, username: "Alice", comment: "Great post!");

        await _commentRepository.CreateAsync(comment);

        var result = await _commentRepository.GetByIdAsync(comment.CommentId);
        Assert.NotNull(result);
        Assert.Equal(comment.CommentId, result.CommentId);
        Assert.Equal("Alice", result.Username);
        Assert.Equal("Great post!", result.Comment);
        Assert.False(result.Edited);
        Assert.Equal(post.PostId, result.PostId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsComment_WhenExists()
    {
        var post = await CreateAndInsertPostAsync();
        var comment = CreateComment(post.PostId);
        await _commentRepository.CreateAsync(comment);

        var result = await _commentRepository.GetByIdAsync(comment.CommentId);

        Assert.NotNull(result);
        Assert.Equal(comment.CommentId, result.CommentId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _commentRepository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCommentFields()
    {
        var post = await CreateAndInsertPostAsync();
        var comment = CreateComment(post.PostId, comment: "Original comment");
        await _commentRepository.CreateAsync(comment);

        comment.Comment = "Edited comment";
        comment.Edited = true;
        await _commentRepository.UpdateAsync(comment);

        var result = await _commentRepository.GetByIdAsync(comment.CommentId);
        Assert.NotNull(result);
        Assert.Equal("Edited comment", result.Comment);
        Assert.True(result.Edited);
    }

    [Fact]
    public async Task UpdateAsync_DoesNotAffectOtherComments()
    {
        var post = await CreateAndInsertPostAsync();
        var comment1 = CreateComment(post.PostId, comment: "Comment 1");
        var comment2 = CreateComment(post.PostId, comment: "Comment 2");
        await _commentRepository.CreateAsync(comment1);
        await _commentRepository.CreateAsync(comment2);

        comment1.Comment = "Updated Comment 1";
        await _commentRepository.UpdateAsync(comment1);

        var result2 = await _commentRepository.GetByIdAsync(comment2.CommentId);
        Assert.NotNull(result2);
        Assert.Equal("Comment 2", result2.Comment);
    }

    [Fact]
    public async Task DeleteAsync_RemovesComment_WhenExists()
    {
        var post = await CreateAndInsertPostAsync();
        var comment = CreateComment(post.PostId);
        await _commentRepository.CreateAsync(comment);

        await _commentRepository.DeleteAsync(comment.CommentId);

        var result = await _commentRepository.GetByIdAsync(comment.CommentId);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_IsNoOp_WhenNotExists()
    {
        var exception = await Record.ExceptionAsync(() =>
            _commentRepository.DeleteAsync(Guid.NewGuid()));

        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotDeleteParentPost()
    {
        var post = await CreateAndInsertPostAsync();
        var comment = CreateComment(post.PostId);
        await _commentRepository.CreateAsync(comment);

        await _commentRepository.DeleteAsync(comment.CommentId);

        var parentPost = await _postRepository.GetByIdAsync(post.PostId);
        Assert.NotNull(parentPost);
    }
}
