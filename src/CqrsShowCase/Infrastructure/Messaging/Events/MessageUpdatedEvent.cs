using CqrsShowCase.Application.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Events;

public class MessageUpdatedEvent : BaseEvent
{
    public MessageUpdatedEvent() : base(nameof(MessageUpdatedEvent))
    {
    }

    public string Message { get; set; }
}