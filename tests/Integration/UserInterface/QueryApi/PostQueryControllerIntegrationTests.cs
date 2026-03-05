using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CqrsShowCase.Infrastructure.Data.MsSqlServer.DataAccess;
using CqrsShowCase.Infrastructure.Repositories;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.UserInterface.QueryApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CqrsShowCase.Tests.Integration.UserInterface.QueryApi;

public class PostQueryControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DatabaseContextFactory _contextFactory;
    private readonly PostRepository _postRepository;
    private readonly CommentRepository _commentRepository;

    public PostQueryControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _contextFactory = factory.Services.GetRequiredService<DatabaseContextFactory>();
        _postRepository = new PostRepository(_contextFactory);
        _commentRepository = new CommentRepository(_contextFactory);

        using var context = _contextFactory.CreateDbContext();
        context.Database.ExecuteSqlRaw("DELETE FROM [Comment]");
        context.Database.ExecuteSqlRaw("DELETE FROM [Post]");
    }

    private static PostEntity CreatePost(string author = "Test Author", int likes = 0, string message = "Test message")
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

    private static CommentEntity CreateComment(Guid postId, string username = "commenter", string comment = "Test comment")
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
    public async Task GetAllPostsAsync_ReturnsOk_WhenPostsExist()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Alice"));
        await _postRepository.CreateAsync(CreatePost(author: "Bob"));
        await _postRepository.CreateAsync(CreatePost(author: "Carol"));

        var response = await _client.GetAsync("api/v1/PostQuery");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();
        Assert.NotNull(body);
        Assert.Equal(3, body.Posts.Count);
        Assert.Equal("Successfully returned 3 posts!", body.Message);
    }

    [Fact]
    public async Task GetAllPostsAsync_ReturnsNoContent_WhenNoPosts()
    {
        var response = await _client.GetAsync("api/v1/PostQuery");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetPostByIdAsync_ReturnsOk_WhenExists()
    {
        var post = CreatePost(author: "Robert Martin", message: "Clean code matters.");
        await _postRepository.CreateAsync(post);

        var response = await _client.GetAsync($"api/v1/PostQuery/byId/{post.PostId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Posts);
        Assert.Equal(post.PostId, body.Posts[0].PostId);
        Assert.Equal("Robert Martin", body.Posts[0].Author);
        Assert.Equal("Successfully returned 1 post!", body.Message);
    }

    [Fact]
    public async Task GetPostByIdAsync_ReturnsNoContent_WhenNotFound()
    {
        var response = await _client.GetAsync($"api/v1/PostQuery/byId/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetPostsByAuthorAsync_ReturnsOk_WhenAuthorMatches()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Robert Martin"));
        await _postRepository.CreateAsync(CreatePost(author: "Alice Johnson"));

        var response = await _client.GetAsync("api/v1/PostQuery/byAuthor/Martin");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Posts);
        Assert.Equal("Robert Martin", body.Posts[0].Author);
        Assert.Equal("Successfully returned 1 post!", body.Message);
    }

    [Fact]
    public async Task GetPostsByAuthorAsync_ReturnsNoContent_WhenNoMatch()
    {
        await _postRepository.CreateAsync(CreatePost(author: "Alice"));

        var response = await _client.GetAsync("api/v1/PostQuery/byAuthor/Unknown");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetPostsWithCommentsAsync_ReturnsOk_WhenPostsHaveComments()
    {
        var postWithComment = CreatePost(author: "Alice");
        var postWithoutComment = CreatePost(author: "Bob");
        await _postRepository.CreateAsync(postWithComment);
        await _postRepository.CreateAsync(postWithoutComment);
        await _commentRepository.CreateAsync(CreateComment(postWithComment.PostId));

        var response = await _client.GetAsync("api/v1/PostQuery/withComments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();
        Assert.NotNull(body);
        Assert.Single(body.Posts);
        Assert.Equal("Alice", body.Posts[0].Author);
        Assert.Equal("Successfully returned 1 post!", body.Message);
    }

    [Fact]
    public async Task GetPostsWithCommentsAsync_ReturnsNoContent_WhenNoPostsHaveComments()
    {
        await _postRepository.CreateAsync(CreatePost());

        var response = await _client.GetAsync("api/v1/PostQuery/withComments");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetPostsWithLikesAsync_ReturnsOk_WhenThresholdMet()
    {
        await _postRepository.CreateAsync(CreatePost(author: "A", likes: 2));
        await _postRepository.CreateAsync(CreatePost(author: "B", likes: 5));
        await _postRepository.CreateAsync(CreatePost(author: "C", likes: 10));

        var response = await _client.GetAsync("api/v1/PostQuery/withLikes/5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();
        Assert.NotNull(body);
        Assert.Equal(2, body.Posts.Count);
        Assert.All(body.Posts, p => Assert.True(p.Likes >= 5));
        Assert.Equal("Successfully returned 2 posts!", body.Message);
    }

    [Fact]
    public async Task GetPostsWithLikesAsync_ReturnsNoContent_WhenNoPostsMeetThreshold()
    {
        await _postRepository.CreateAsync(CreatePost(likes: 3));

        var response = await _client.GetAsync("api/v1/PostQuery/withLikes/5");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllPostsAsync_MessageUsesPlural_WhenMultiplePosts()
    {
        await _postRepository.CreateAsync(CreatePost());
        await _postRepository.CreateAsync(CreatePost());

        var response = await _client.GetAsync("api/v1/PostQuery");
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();

        Assert.NotNull(body);
        Assert.Equal("Successfully returned 2 posts!", body.Message);
    }

    [Fact]
    public async Task GetAllPostsAsync_MessageUsesSingular_WhenOnePost()
    {
        await _postRepository.CreateAsync(CreatePost());

        var response = await _client.GetAsync("api/v1/PostQuery");
        var body = await response.Content.ReadFromJsonAsync<PostQueryResponse>();

        Assert.NotNull(body);
        Assert.Equal("Successfully returned 1 post!", body.Message);
    }
}
