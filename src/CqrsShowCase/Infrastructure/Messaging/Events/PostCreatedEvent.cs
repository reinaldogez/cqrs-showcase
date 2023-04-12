using CqrsShowCase.Application.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class PostCreatedEvent : BaseEvent
{
    protected PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }

    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime DatePosted { get; set; }
}