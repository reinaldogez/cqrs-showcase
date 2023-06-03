using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Core.Infrascructure;
public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
    Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
}