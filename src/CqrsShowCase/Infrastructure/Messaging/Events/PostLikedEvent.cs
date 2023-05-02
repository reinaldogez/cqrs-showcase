using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class PostLikedEvent : BaseEvent
{
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }
}