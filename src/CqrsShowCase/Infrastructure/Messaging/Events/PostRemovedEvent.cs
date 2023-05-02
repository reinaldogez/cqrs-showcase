using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}