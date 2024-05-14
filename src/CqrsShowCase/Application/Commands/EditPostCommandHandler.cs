using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CqrsShowCase.Command.Domain.Aggregates;
using CqrsShowCase.Core.Handlers;

namespace CqrsShowCase.Application.Commands;

public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public EditCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task Handle(EditCommentCommand command, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.Username);

        await _eventSourcingHandler.SaveAsync(aggregate);
    }
}
