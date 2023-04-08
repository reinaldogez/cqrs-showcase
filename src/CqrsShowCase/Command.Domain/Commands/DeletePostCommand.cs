namespace CqrsShowCase.Command.Domain.Commands;

public class DeletePostCommand : BaseCommand
{
    public string Username { get; set; }
}