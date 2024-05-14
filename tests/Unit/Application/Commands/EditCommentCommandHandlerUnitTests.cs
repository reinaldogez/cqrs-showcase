using Xunit;
using Moq;
using CqrsShowCase.Application.Commands;
using CqrsShowCase.Core.Handlers;
using CqrsShowCase.Command.Domain.Aggregates;
using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Tests.Unit.Application.Commands;

public class EditCommentCommandHandlerUnitTests
{
    [Fact]
    public async Task EditCommentCommandHandler_WhenHandleIsCalled_ShouldInvokeEventSourcingHandlerMethodsCorrectly()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PostAggregate>>();
        var postId = Guid.NewGuid();
        var username = "testuser";
        var newComment = "Edited Comment";

        var postAggregate = new PostAggregate(postId, "author", "Initial Message");
        postAggregate.AddComment("Original Comment", username);
        var commentId = postAggregate.GetUncommittedChanges().OfType<CommentAddedEvent>().Last().CommentId;

        mockEventSourcingHandler.Setup(m => m.GetByIdAsync(postId))
            .ReturnsAsync(postAggregate);

        var handler = new EditCommentCommandHandler(mockEventSourcingHandler.Object);
        var command = new EditCommentCommand
        {
            Id = postId,
            CommentId = commentId,
            Comment = newComment,
            Username = username
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockEventSourcingHandler.Verify(m => m.GetByIdAsync(postId), Times.Once, "GetByIdAsync was not called exactly once with the correct parameters.");
        mockEventSourcingHandler.Verify(m => m.SaveAsync(It.IsAny<PostAggregate>()), Times.Once, "SaveAsync was not called exactly once.");
    }

    [Fact]
    public async Task EditCommentCommandHandler_WhenHandleIsCalled_ShouldCorrectlyApplyCommentEdit()
    {
        // Arrange
        var mockEventSourcingHandler = new Mock<IEventSourcingHandler<PostAggregate>>();
        var postId = Guid.NewGuid();
        var username = "testuser";
        var originalComment = "Original Comment";
        var newComment = "Edited Comment";

        var postAggregate = new PostAggregate(postId, "author", "Initial Message");
        postAggregate.AddComment(originalComment, username);
        var commentId = postAggregate.GetUncommittedChanges().OfType<CommentAddedEvent>().Last().CommentId;

        postAggregate.MarkChangesAsCommitted();  // Clear initial events to focus on new changes.

        mockEventSourcingHandler.Setup(m => m.GetByIdAsync(postId))
            .ReturnsAsync(postAggregate);

        var handler = new EditCommentCommandHandler(mockEventSourcingHandler.Object);
        var command = new EditCommentCommand
        {
            Id = postId,
            CommentId = commentId,
            Comment = newComment,
            Username = username
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var changes = postAggregate.GetUncommittedChanges().ToList();
        Assert.Single(changes);
        Assert.IsType<CommentUpdatedEvent>(changes.Single());
        var updateEvent = (CommentUpdatedEvent)changes.Single();
        Assert.Equal(commentId, updateEvent.CommentId);
        Assert.Equal(newComment, updateEvent.Comment);
        Assert.Equal(username, updateEvent.Username);
    }
}