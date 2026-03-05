using Moq;
using CqrsShowCase.Application.Queries;
using CqrsShowCase.Infrastructure.Handlers;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;

namespace CqrsShowCase.Tests.Unit.Infrastructure.Handlers;

public class FindAllPostsQueryHandlerUnitTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly FindAllPostsQueryHandler _handler;

    public FindAllPostsQueryHandlerUnitTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _handler = new FindAllPostsQueryHandler(_mockPostRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllPosts()
    {
        // Arrange
        var expectedPosts = new List<PostEntity>
        {
            new PostEntity { PostId = Guid.NewGuid(), Author = "Alice", Message = "Hello", DatePosted = DateTime.UtcNow },
            new PostEntity { PostId = Guid.NewGuid(), Author = "Bob", Message = "World", DatePosted = DateTime.UtcNow }
        };
        _mockPostRepository.Setup(r => r.ListAllAsync()).ReturnsAsync(expectedPosts);

        // Act
        var result = await _handler.Handle(new FindAllPostsQuery(), CancellationToken.None);

        // Assert
        _mockPostRepository.Verify(r => r.ListAllAsync(), Times.Once);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WhenNoPosts_ShouldReturnEmptyList()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.ListAllAsync()).ReturnsAsync(new List<PostEntity>());

        // Act
        var result = await _handler.Handle(new FindAllPostsQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}

public class FindPostByIdQueryHandlerUnitTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly FindPostByIdQueryHandler _handler;

    public FindPostByIdQueryHandlerUnitTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _handler = new FindPostByIdQueryHandler(_mockPostRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenPostExists_ShouldReturnListWithPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var expectedPost = new PostEntity { PostId = postId, Author = "Alice", Message = "Hello", DatePosted = DateTime.UtcNow };
        _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(expectedPost);

        // Act
        var result = await _handler.Handle(new FindPostByIdQuery { Id = postId }, CancellationToken.None);

        // Assert
        _mockPostRepository.Verify(r => r.GetByIdAsync(postId), Times.Once);
        Assert.Single(result);
        Assert.Equal(postId, result[0].PostId);
    }

    [Fact]
    public async Task Handle_WhenPostDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _mockPostRepository.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((PostEntity)null);

        // Act
        var result = await _handler.Handle(new FindPostByIdQuery { Id = postId }, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}

public class FindPostsByAuthorQueryHandlerUnitTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly FindPostsByAuthorQueryHandler _handler;

    public FindPostsByAuthorQueryHandlerUnitTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _handler = new FindPostsByAuthorQueryHandler(_mockPostRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectAuthor()
    {
        // Arrange
        var author = "Alice";
        var expectedPosts = new List<PostEntity>
        {
            new PostEntity { PostId = Guid.NewGuid(), Author = author, Message = "Hello", DatePosted = DateTime.UtcNow }
        };
        _mockPostRepository.Setup(r => r.ListByAuthorAsync(author)).ReturnsAsync(expectedPosts);

        // Act
        var result = await _handler.Handle(new FindPostsByAuthorQuery { Author = author }, CancellationToken.None);

        // Assert
        _mockPostRepository.Verify(r => r.ListByAuthorAsync(author), Times.Once);
        Assert.Single(result);
        Assert.Equal(author, result[0].Author);
    }

    [Fact]
    public async Task Handle_WhenNoPostsForAuthor_ShouldReturnEmptyList()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.ListByAuthorAsync(It.IsAny<string>())).ReturnsAsync(new List<PostEntity>());

        // Act
        var result = await _handler.Handle(new FindPostsByAuthorQuery { Author = "Unknown" }, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}

public class FindPostsWithCommentsQueryHandlerUnitTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly FindPostsWithCommentsQueryHandler _handler;

    public FindPostsWithCommentsQueryHandlerUnitTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _handler = new FindPostsWithCommentsQueryHandler(_mockPostRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPostsWithComments()
    {
        // Arrange
        var expectedPosts = new List<PostEntity>
        {
            new PostEntity
            {
                PostId = Guid.NewGuid(),
                Author = "Alice",
                Message = "Hello",
                DatePosted = DateTime.UtcNow,
                Comments = new List<CommentEntity>
                {
                    new CommentEntity { CommentId = Guid.NewGuid(), Username = "Bob", Comment = "Nice!", CommentDate = DateTime.UtcNow }
                }
            }
        };
        _mockPostRepository.Setup(r => r.ListWithCommentsAsync()).ReturnsAsync(expectedPosts);

        // Act
        var result = await _handler.Handle(new FindPostsWithCommentsQuery(), CancellationToken.None);

        // Assert
        _mockPostRepository.Verify(r => r.ListWithCommentsAsync(), Times.Once);
        Assert.Single(result);
        Assert.NotEmpty(result[0].Comments);
    }

    [Fact]
    public async Task Handle_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.ListWithCommentsAsync()).ReturnsAsync(new List<PostEntity>());

        // Act
        var result = await _handler.Handle(new FindPostsWithCommentsQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}

public class FindPostsWithLikesQueryHandlerUnitTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly FindPostsWithLikesQueryHandler _handler;

    public FindPostsWithLikesQueryHandlerUnitTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _handler = new FindPostsWithLikesQueryHandler(_mockPostRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectLikesCount()
    {
        // Arrange
        var numberOfLikes = 5;
        var expectedPosts = new List<PostEntity>
        {
            new PostEntity { PostId = Guid.NewGuid(), Author = "Alice", Message = "Popular!", Likes = 10, DatePosted = DateTime.UtcNow }
        };
        _mockPostRepository.Setup(r => r.ListWithLikesAsync(numberOfLikes)).ReturnsAsync(expectedPosts);

        // Act
        var result = await _handler.Handle(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes }, CancellationToken.None);

        // Assert
        _mockPostRepository.Verify(r => r.ListWithLikesAsync(numberOfLikes), Times.Once);
        Assert.Single(result);
        Assert.True(result[0].Likes >= numberOfLikes);
    }

    [Fact]
    public async Task Handle_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.ListWithLikesAsync(It.IsAny<int>())).ReturnsAsync(new List<PostEntity>());

        // Act
        var result = await _handler.Handle(new FindPostsWithLikesQuery { NumberOfLikes = 100 }, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
