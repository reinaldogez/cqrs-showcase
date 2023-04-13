using CqrsShowCase.Command.Domain;

namespace CqrsShowCase.Application.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Author { get; set; }
    public string Message { get; set; }
}