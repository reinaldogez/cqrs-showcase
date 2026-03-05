using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CqrsShowCase.Infrastructure.Repositories;
using CqrsShowCase.Query.Domain.Entities;
using Xunit;

namespace CqrsShowCase.Tests.Integration.Infrastructure.Repositories;

[Collection("SqlServer")]
public class PostRepositoryIntegrationTests
{
    private readonly SqlServerTestFixture _fixture;
    private readonly PostRepository _postRepository;
    private readonly CommentRepository _commentRepository;

    public PostRepositoryIntegrationTests(SqlServerTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.CleanDatabaseAsync().GetAwaiter().GetResult();
        _postRepository = new PostRepository(_fixture.ContextFactory);
        _commentRepository = new CommentRepository(_fixture.ContextFactory);
    }

    private static PostEntity CreatePost(string author = "John Doe", int likes = 0, string message = "Test message")
    {
        return new PostEntity
        {
            PostId = Guid.NewGuid(),
            Author = author,
            DatePosted = DateTime.UtcNow,
            Message = message,
            Likes = likes
        };
    }

    private static CommentEntity CreateComment(Guid postId, string username = "commenter", string comment = "Nice post!")
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
    public async Task CreateAsync_PersistsPostToDatabase()
    {
        var post = CreatePost(author: "Robert Martin", likes: 5, message: "Clean code matters.");

        await _postRepository.CreateAsync(post);

        var result = await _postRepository.GetByIdAsync(post.PostId);
        Assert.NotNull(result);
        Assert.Equal(post.PostId, result.PostId);
        Assert.Equal("Robert Martin", result.Author);
        Assert.Equal("Clean code matters.", result.Message);
        Assert.Equal(5, result.Likes);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPost_WhenExists()
    {
        var post = CreatePost();
        await _postRepository.CreateAsync(post);

        var result = await _postRepository.GetByIdAsync(post.PostId);

        Assert.NotNull(result);
        Assert.Equal(post.PostId, result.PostId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _postRepository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingPost()
    {
        var post = CreatePost(message: "Original message", likes: 0);
        await _postRepository.CreateAsync(post);

        post.Message = "Updated message";
        post.Likes = 10;
        await _postRepository.UpdateAsync(post);

        var result = await _postRepository.GetByIdAsync(post.PostId);
        Assert.NotNull(result);
        Assert.Equal("Updated message", result.Message);
        Assert.Equal(10, result.Likes);
    }

    [Fact]
    public async Task DeleteAsync_RemovesPost_WhenExists()
    {
        var post = CreatePost();
        await _postRepository.CreateAsync(post);

        await _postRepository.DeleteAsync(post.PostId);

        var result = await _postRepository.GetByIdAsync(post.PostId);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_IsNoOp_WhenNotExists()
    {
        var exception = await Record.ExceptionAsync(() =>
            _postRepository.DeleteAsync(Guid.NewGuid()));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllPosts()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Author1"));
        await _postRepository.CreateAsync(CreatePost(author: "Author2"));
        await _postRepository.CreateAsync(CreatePost(author: "Author3"));

        var result = await _postRepository.ListAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ListAllAsync_ReturnsEmptyList_WhenNoPosts()
    {
        var result = await _postRepository.ListAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListByAuthorAsync_ReturnsMatchingPosts_SubstringMatch()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Robert Martin"));
        await _postRepository.CreateAsync(CreatePost(author: "Alice Johnson"));

        var result = await _postRepository.ListByAuthorAsync("Martin");

        Assert.Single(result);
        Assert.Equal("Robert Martin", result[0].Author);
    }

    [Fact]
    public async Task ListByAuthorAsync_ExcludesNonMatchingPosts()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Alice"));
        await _postRepository.CreateAsync(CreatePost(author: "Bob"));

        var result = await _postRepository.ListByAuthorAsync("Alice");

        Assert.Single(result);
        Assert.Equal("Alice", result[0].Author);
    }

    [Fact]
    public async Task ListByAuthorAsync_ReturnsEmptyList_WhenNoMatch()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Alice"));

        var result = await _postRepository.ListByAuthorAsync("Charlie");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListWithLikesAsync_ReturnsPost_WhenLikesEqualsThreshold()
    {
        await _postRepository.CreateAsync(CreatePost(likes: 5));

        var result = await _postRepository.ListWithLikesAsync(5);

        Assert.Single(result);
    }

    [Fact]
    public async Task ListWithLikesAsync_ExcludesPost_WhenLikesBelowThreshold()
    {
        await _postRepository.CreateAsync(CreatePost(likes: 4));

        var result = await _postRepository.ListWithLikesAsync(5);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ListWithLikesAsync_ReturnsOnlyQualifyingPosts()
    {
        await _postRepository.CreateAsync(CreatePost(author: "A", likes: 2));
        await _postRepository.CreateAsync(CreatePost(author: "B", likes: 5));
        await _postRepository.CreateAsync(CreatePost(author: "C", likes: 10));

        var result = await _postRepository.ListWithLikesAsync(5);

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.Likes >= 5));
    }

    [Fact]
    public async Task ListWithCommentsAsync_ReturnsOnlyPostsWithComments()
    {
        var postWithComments = CreatePost(author: "WithComments");
        var postWithoutComments = CreatePost(author: "WithoutComments");
        await _postRepository.CreateAsync(postWithComments);
        await _postRepository.CreateAsync(postWithoutComments);
        await _commentRepository.CreateAsync(CreateComment(postWithComments.PostId));

        var result = await _postRepository.ListWithCommentsAsync();

        Assert.Single(result);
        Assert.Equal("WithComments", result[0].Author);
    }

    [Fact]
    public async Task ListWithCommentsAsync_IncludesCommentsInResult()
    {
        var post = CreatePost();
        await _postRepository.CreateAsync(post);
        await _commentRepository.CreateAsync(CreateComment(post.PostId, comment: "First comment"));
        await _commentRepository.CreateAsync(CreateComment(post.PostId, comment: "Second comment"));

        var result = await _postRepository.ListWithCommentsAsync();

        Assert.Single(result);
        Assert.NotNull(result[0].Comments);
        Assert.Equal(2, result[0].Comments.Count);
    }

    [Fact]
    public async Task ListWithCommentsAsync_ReturnsEmptyList_WhenNoPostsHaveComments()
    {
        await _postRepository.CreateAsync(CreatePost());
        await _postRepository.CreateAsync(CreatePost());

        var result = await _postRepository.ListWithCommentsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListWithCommentsAsync_ReturnsEmptyList_WhenDatabaseIsEmpty()
    {
        var result = await _postRepository.ListWithCommentsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
