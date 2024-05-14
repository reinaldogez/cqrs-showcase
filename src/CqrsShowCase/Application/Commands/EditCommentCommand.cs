using CqrsShowCase.Command.Domain;
using MediatR;

namespace CqrsShowCase.Application.Commands;

public class EditCommentCommand : BaseCommand, IRequest
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
}