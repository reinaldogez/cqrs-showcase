using CqrsShowCase.Core.Domain;

namespace CqrsShowCase.Core.Handlers;

public interface IEventSourcingHandler<T>
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<T> GetByIdAsync(Guid aggregateId);
}
