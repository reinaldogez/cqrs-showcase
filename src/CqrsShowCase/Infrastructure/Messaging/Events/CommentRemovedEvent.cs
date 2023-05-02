using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class CommentRemovedEvent : BaseEvent
{
    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
    {
    }

    public Guid CommentId { get; set; }
}
