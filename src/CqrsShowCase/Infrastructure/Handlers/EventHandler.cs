using System.Text;
using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    public Task On(PostCreatedEvent @event)
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

        return Task.CompletedTask;
    }

    public Task On(MessageUpdatedEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task On(PostLikedEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task On(CommentAddedEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task On(CommentUpdatedEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task On(CommentRemovedEvent @event)
    {
        throw new NotImplementedException();
    }

    public Task On(PostRemovedEvent @event)
    {
        throw new NotImplementedException();
    }
}