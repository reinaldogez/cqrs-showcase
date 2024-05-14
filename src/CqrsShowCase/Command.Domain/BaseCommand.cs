namespace CqrsShowCase.Command.Domain;

public abstract class BaseCommand
{
    public Guid Id { get; set; }
}