using CqrsShowCase.Application.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}