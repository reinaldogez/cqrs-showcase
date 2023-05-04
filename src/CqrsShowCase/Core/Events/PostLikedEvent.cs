namespace CqrsShowCase.Core.Events;

public class PostLikedEvent : BaseEvent
{
    public PostLikedEvent() : base(nameof(PostLikedEvent))
    {
    }
}