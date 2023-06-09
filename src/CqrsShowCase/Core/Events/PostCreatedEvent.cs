namespace CqrsShowCase.Core.Events;

public class PostCreatedEvent : BaseEvent
{
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }

    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime DatePosted { get; set; }
}