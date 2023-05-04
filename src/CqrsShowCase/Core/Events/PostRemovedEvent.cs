namespace CqrsShowCase.Core.Events;

public class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}