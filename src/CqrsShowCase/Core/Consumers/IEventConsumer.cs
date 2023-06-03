namespace CqrsShowCase.Core.Consumers;

public interface IEventConsumer
{
    void Consume(string topic, CancellationToken cancellationToken);
}
