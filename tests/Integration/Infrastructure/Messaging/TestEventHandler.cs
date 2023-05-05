using System.Threading.Tasks;
using CqrsShowCase.Application.Handlers;
using CqrsShowCase.Core.Events;

namespace PocKafka.Tests.Integration;

public class TestEventHandler : IEventHandler
{
    public PostCreatedEvent HandledEvent { get; private set; }
    public TaskCompletionSource<PostCreatedEvent> EventHandledCompletionSource { get; } = new TaskCompletionSource<PostCreatedEvent>();

    public Task On(PostCreatedEvent @event)
    {
        HandledEvent = @event;
        EventHandledCompletionSource.SetResult(@event);
        return Task.CompletedTask;
    }

    public Task On(MessageUpdatedEvent @event)
    {
        throw new System.NotImplementedException();
    }

    public Task On(PostLikedEvent @event)
    {
        throw new System.NotImplementedException();
    }

    public Task On(CommentAddedEvent @event)
    {
        throw new System.NotImplementedException();
    }

    public Task On(CommentUpdatedEvent @event)
    {
        throw new System.NotImplementedException();
    }

    public Task On(CommentRemovedEvent @event)
    {
        throw new System.NotImplementedException();
    }

    public Task On(PostRemovedEvent @event)
    {
        throw new System.NotImplementedException();
    }

}