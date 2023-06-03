using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Core.Producers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
}