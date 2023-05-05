using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CqrsShowCase.Core.Events;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace PocKafka.Tests.Integration;

public class EventConsumerIntegrationTests : IClassFixture<KafkaTestServerFixture>
{
    private readonly KafkaTestServerFixture _fixture;
    private readonly EventProducer _eventProducer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly ITestOutputHelper _output;

    public EventConsumerIntegrationTests(
        KafkaTestServerFixture fixture,
        ITestOutputHelper output)
    {
        _fixture = fixture;
        _eventProducer = _fixture.ServiceProvider.GetRequiredService<EventProducer>();
        _consumerConfig = _fixture.ServiceProvider.GetRequiredService<ConsumerConfig>();
        _output = output;
    }

    [Fact]
    public async Task EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully()
    {
        // Arrange
        var testTopicName = $"test-{Guid.NewGuid()}";
        _output.WriteLine($"Test topic name created on" +
            $" EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully method: {testTopicName}");

        var testEvent = new PostCreatedEvent
        {
            Id = Guid.NewGuid(),
            Author = "Robert C. Martin",
            DatePosted = DateTime.Now,
            Message = "Clean code always looks like it was written by someone who cares.",
            Type = nameof(PostCreatedEvent),
            Version = 0
        };
        // Create a CancellationTokenSource with a timeout of 30 seconds
        using CancellationTokenSource timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        CancellationTokenSource cts = new CancellationTokenSource();
        Task consumerTask = null;
        try
        {
            var testEventHandler = new TestEventHandler();
            var eventConsumer = new EventConsumer(_consumerConfig, testEventHandler);

            // Act
            await _eventProducer.ProduceAsync(testTopicName, testEvent);
            consumerTask = Task.Run(() => eventConsumer.Consume(testTopicName, cts.Token));
            await testEventHandler.EventHandledCompletionSource.Task.WaitAsync(timeoutCts.Token);

            // Assert
            Assert.NotNull(testEventHandler.HandledEvent);
            Assert.Equal(testEvent.Id, testEventHandler.HandledEvent.Id);
        }
        catch (TaskCanceledException ex)
        {
            _output.WriteLine($"Exception on EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully: {Environment.NewLine}" +
                $"The operation timed out {ex.Message}");
            throw new Exception("The operation timed out", ex);
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Exception on EventConsumer_ConsumesAndHandlesPostCreatedEvent_Successfully{ex.Message}");
        }
        finally
        {
            //the broker needs to ensure that there are no active consumers or producers using
            //the topic before it can safely remove it.
            cts.Cancel();

            try
            {
                // Wait for the consumer to finish
                await consumerTask;
            }
            catch (OperationCanceledException)
            {
                // Ignore the exception since we're cancelling the consumer deliberately
            }

            // Clean up the test topic
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _consumerConfig.BootstrapServers }).Build())
            {
                _output.WriteLine($"Deleting test topic: {testTopicName}");
                await adminClient.DeleteTopicsAsync(new List<string> { testTopicName });
            }
        }
    }
}
