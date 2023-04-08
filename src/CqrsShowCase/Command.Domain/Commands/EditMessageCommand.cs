namespace CqrsShowCase.Command.Domain.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Author { get; set; }
    public string Message { get; set; }
}