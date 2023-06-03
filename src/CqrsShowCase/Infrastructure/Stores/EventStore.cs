using CqrsShowCase.Core.Consumers;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Core.Infrascructure;
using CqrsShowCase.Core.Producers;

namespace CqrsShowCase.Infrastructure.Stores;

public class EventStore : IEventStore
{
    // private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventProducer eventProducer)
    {
        _eventProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        // Do this method with the Mongo DB feature
        // simply return a task with an empty list for while
        return await Task.FromResult(new List<BaseEvent>());
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        foreach (var @event in events)
        {
            @event.Version = 0;

            string topicName = Environment.GetEnvironmentVariable("KAFKA_TOPIC", EnvironmentVariableTarget.User);
            await _eventProducer.ProduceAsync(topicName, @event);
        }
    }
}
