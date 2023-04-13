using CqrsShowCase.Command.Domain;

namespace CqrsShowCase.Application.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}