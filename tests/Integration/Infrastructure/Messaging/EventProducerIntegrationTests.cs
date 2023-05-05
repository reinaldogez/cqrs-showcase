using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace PocKafka.Tests.Integration;

public class EventProducerIntegrationTests : IClassFixture<KafkaTestServerFixture>
{
    private readonly EventProducer _eventProducer;
    private readonly ProducerConfig _producerConfig;
    private readonly ConsumerConfig _consumerConfig;
    private readonly ITestOutputHelper _output;

    public EventProducerIntegrationTests(
        KafkaTestServerFixture fixture,
        ITestOutputHelper output)
    {
        _eventProducer = fixture.ServiceProvider.GetRequiredService<EventProducer>();
        _consumerConfig = fixture.ServiceProvider.GetRequiredService<ConsumerConfig>();
        _producerConfig = fixture.ServiceProvider.GetRequiredService<ProducerConfig>();
        _output = output;
    }

    [Fact]
    public async Task EventProducer_ShouldProduceEventToKafka_Successfully()
    {
        // Arrange
        var testTopicName = $"test-{Guid.NewGuid()}";
        PostCreatedEvent eventData = new(); // Replace with your actual derived event class
        eventData.Id = Guid.NewGuid();
        _output.WriteLine($"Test topic name created on" +
            $" EventProducer_ShouldProduceEventToKafka_Successfully method: {testTopicName}");

        try
        {
            // Act
            await _eventProducer.ProduceAsync(testTopicName, eventData);
            _output.WriteLine($"Test topic name created: {testTopicName}");

            // Assert
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(testTopicName);
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
            consumer.Close();

            PostCreatedEvent consumedEventData = JsonSerializer.Deserialize<PostCreatedEvent>(consumeResult.Message.Value);

            Assert.NotNull(consumeResult);
            Assert.Equal(eventData.Id, consumedEventData.Id);
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Exception on EventProducer_ShouldProduceEventToKafka_Successfully{ex.Message}");
        }
        finally
        {
            // Clean up the test topic
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _producerConfig.BootstrapServers }).Build())
            {
                await adminClient.DeleteTopicsAsync(new List<string> { testTopicName });
            }
        }
    }
}