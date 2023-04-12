using CqrsShowCase.Application.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class PostLikedEvent : BaseEvent
{
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }
}