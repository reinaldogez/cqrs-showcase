using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Core.Domain;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @event);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
}