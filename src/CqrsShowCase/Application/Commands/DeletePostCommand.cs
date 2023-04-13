using CqrsShowCase.Command.Domain;

namespace CqrsShowCase.Application.Commands;

public class DeletePostCommand : BaseCommand
{
    public string Username { get; set; }
}