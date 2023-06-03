using System.Text;
using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Infrastructure.Data.AzureCosmosDb;
using CqrsShowCase.Query.Domain.Entities;
using CqrsShowCase.Query.Domain.Repositories;

namespace CqrsShowCase.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task On(PostCreatedEvent @event)
    {
        StringBuilder sb = new();
        sb.AppendLine($"EventType: {@event.Type}");
        sb.AppendLine($"PostId: {@event.Id}");
        sb.AppendLine($"Author: {@event.Author}");
        sb.AppendLine($"DatePosted: {@event.DatePosted}");
        sb.AppendLine($"Message: {@event.Message}");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(sb.ToString());
        Console.ResetColor();
        
        var post = new CosmosPostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };

        await _postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await _postRepository.GetByIdAsync(@event.Id);

        if (post == null) return;

        post.Message = @event.Message;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await _postRepository.GetByIdAsync(@event.Id);

        if (post == null) return;

        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            Username = @event.Username,
            Edited = false
        };

        await _commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await _commentRepository.GetByIdAsync(@event.CommentId);

        if (comment == null) return;

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await _commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        await _commentRepository.DeleteAsync(@event.CommentId);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await _postRepository.DeleteAsync(@event.Id);
    }
}