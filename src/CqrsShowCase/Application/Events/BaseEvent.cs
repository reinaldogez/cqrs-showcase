namespace CqrsShowCase.Application.Events;

public abstract class BaseEvent
{
    protected BaseEvent(string type){
        this.Type = type;
    }
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Type { get; set; }
}
