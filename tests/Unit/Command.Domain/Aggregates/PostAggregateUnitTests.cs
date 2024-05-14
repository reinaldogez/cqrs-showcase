using Xunit;
using System;
using System.Linq;
using CqrsShowCase.Command.Domain.Aggregates;
using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Tests.Unit.Command.Domain.Aggregates;

public class PostAggregateTests
{
    [Fact]
    public void PostAggregate_WhenCreated_ShouldRaisePostCreatedEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var author = "Author";
        var message = "Initial message";

        // Act
        var post = new PostAggregate(id, author, message);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Single(changes);
        var postCreatedEvent = Assert.IsType<PostCreatedEvent>(changes.First());
        Assert.Equal(id, postCreatedEvent.Id);
        Assert.Equal(author, postCreatedEvent.Author);
        Assert.Equal(message, postCreatedEvent.Message);
        Assert.True(post.Active);
    }

    [Fact]
    public void PostAggregate_WhenMessageEdited_ShouldRaiseMessageUpdatedEvent()
    {
        // Arrange
        var post = GivenAPost();

        var newMessage = "Updated message";

        // Act
        post.EditMessage(newMessage);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(2, changes.Count); // PostCreatedEvent and MessageUpdatedEvent
        var messageUpdatedEvent = Assert.IsType<MessageUpdatedEvent>(changes.Last());
        Assert.Equal(newMessage, messageUpdatedEvent.Message);
    }

    [Fact]
    public void PostAggregate_WhenLiked_ShouldRaisePostLikedEvent()
    {
        // Arrange
        var post = GivenAPost();

        // Act
        post.LikePost();

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(2, changes.Count); // PostCreatedEvent and PostLikedEvent
        var postLikedEvent = Assert.IsType<PostLikedEvent>(changes.Last());
        Assert.Equal(post.Id, postLikedEvent.Id);
    }

    [Fact]
    public void PostAggregate_WhenCommentAdded_ShouldRaiseCommentAddedEvent()
    {
        // Arrange
        var post = GivenAPost();
        var comment = "This is a comment";
        var username = "Commenter";

        // Act
        post.AddComment(comment, username);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(2, changes.Count); // PostCreatedEvent and CommentAddedEvent
        var commentAddedEvent = Assert.IsType<CommentAddedEvent>(changes.Last());
        Assert.Equal(comment, commentAddedEvent.Comment);
        Assert.Equal(username, commentAddedEvent.Username);
    }

    [Fact]
    public void PostAggregate_WhenCommentEdited_ShouldRaiseCommentUpdatedEvent()
    {
        // Arrange
        var post = GivenAPost();
        var comment = "This is a comment";
        var username = "Commenter";
        post.AddComment(comment, username);
        var commentId = post.GetUncommittedChanges().OfType<CommentAddedEvent>().Last().CommentId;
        var editedComment = "Edited comment";

        // Act
        post.EditComment(commentId, editedComment, username);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(3, changes.Count); // PostCreatedEvent, CommentAddedEvent, and CommentUpdatedEvent
        var commentUpdatedEvent = Assert.IsType<CommentUpdatedEvent>(changes.Last());
        Assert.Equal(editedComment, commentUpdatedEvent.Comment);
        Assert.Equal(username, commentUpdatedEvent.Username);
    }

    [Fact]
    public void PostAggregate_WhenCommentRemoved_ShouldRaiseCommentRemovedEvent()
    {
        // Arrange
        var post = GivenAPost();
        var comment = "This is a comment";
        var username = "Commenter";
        post.AddComment(comment, username);
        var commentId = post.GetUncommittedChanges().OfType<CommentAddedEvent>().Last().CommentId;

        // Act
        post.RemoveComment(commentId, username);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(3, changes.Count); // PostCreatedEvent, CommentAddedEvent, and CommentRemovedEvent
        var commentRemovedEvent = Assert.IsType<CommentRemovedEvent>(changes.Last());
        Assert.Equal(commentId, commentRemovedEvent.CommentId);
    }

    [Fact]
    public void PostAggregate_WhenDeleted_ShouldRaisePostRemovedEvent()
    {
        // Arrange
        var post = GivenAPost();
        var username = "Author"; // The author can delete the post

        // Act
        post.DeletePost(username);

        // Assert
        var changes = post.GetUncommittedChanges().ToList();
        Assert.Equal(2, changes.Count); // PostCreatedEvent and PostRemovedEvent
        var postRemovedEvent = Assert.IsType<PostRemovedEvent>(changes.Last());
        Assert.Equal(post.Id, postRemovedEvent.Id);
        Assert.False(post.Active);
    }

     [Fact]
    public void PostAggregate_WhenEditMessageOnInactivePost_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        post.DeletePost("Author"); // This action will set the post to inactive.
        var newMessage = "Updated message";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.EditMessage(newMessage));
        Assert.Equal("You cannot edit the message of an inactive post!", exception.Message);
    }

    [Fact]
    public void PostAggregate_WhenEditMessageWithEmptyValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        var newMessage = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.EditMessage(newMessage));
        Assert.Equal($"The value of message cannot be null or empty. Please provide a valid message!", exception.Message);
    }

    [Fact]
    public void PostAggregate_WhenAddCommentToInactivePost_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        post.DeletePost("Author"); // This action will set the post to inactive.
        var comment = "This is a comment";
        var username = "Commenter";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.AddComment(comment, username));
        Assert.Equal("You cannot add a comment to an inactive post!", exception.Message);
    }

    [Fact]
    public void PostAggregate_WhenAddEmptyComment_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        var comment = string.Empty;
        var username = "Commenter";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.AddComment(comment, username));
        Assert.Equal($"The value of comment cannot be null or empty. Please provide a valid comment!", exception.Message);
    }

    [Fact]
    public void PostAggregate_WhenRemoveCommentByNonAuthor_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        var comment = "This is a comment";
        var authorUsername = "Author";
        var otherUsername = "OtherUser";
        post.AddComment(comment, authorUsername);
        var commentId = post.GetUncommittedChanges().OfType<CommentAddedEvent>().Last().CommentId;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.RemoveComment(commentId, otherUsername));
        Assert.Equal("You are not allowed to remove a comment that was made by another user!", exception.Message);
    }

    [Fact]
    public void PostAggregate_WhenDeletePostByNonAuthor_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var post = GivenAPost();
        var otherUsername = "OtherUser";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => post.DeletePost(otherUsername));
        Assert.Equal("You are not allowed to delete a post that was made by someone else!", exception.Message);
    }

    private PostAggregate GivenAPost()
    {
        var id = Guid.NewGuid();
        var author = "Author";
        var message = "Initial message";
        var post = new PostAggregate(id, author, message);
        return post;
    }
}
