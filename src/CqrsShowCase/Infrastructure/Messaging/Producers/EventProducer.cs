using System.Text.Json;
using Confluent.Kafka;
using CqrsShowCase.Core.Producers;
using CqrsShowCase.Core.Events;

namespace CqrsShowCase.Infrastructure.Messaging.Consumers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;

    public EventProducer(ProducerConfig config)
    {
        _config = config;
    }

    public async Task ProduceAsync<T>(string topic, T eventObject) where T : BaseEvent
    {
        using var eventProducer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var serializedEvent = JsonSerializer.Serialize(eventObject, eventObject.GetType());

        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = serializedEvent
        };

        var deliveryResult = await eventProducer.ProduceAsync(topic, kafkaMessage);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"Could not produce {eventObject.GetType().Name} message to topic - {topic} due to the following reason: {deliveryResult.Message}.");
        }
        if (deliveryResult.Status == PersistenceStatus.Persisted)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Message persisted. KafkaMessageKey: {kafkaMessage.Key}");
            Console.ResetColor();
        }
    }

}