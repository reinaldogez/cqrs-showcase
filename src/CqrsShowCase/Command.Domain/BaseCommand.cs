namespace CqrsShowCase.Command.Domain;

public abstract class BaseCommand
{
    Guid Id { get; set; }
}