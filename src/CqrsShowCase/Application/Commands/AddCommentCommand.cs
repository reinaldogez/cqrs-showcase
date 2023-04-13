using CqrsShowCase.Command.Domain;

namespace CqrsShowCase.Application.Commands;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}