using System;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using Xunit;
using CqrsShowCase.Infrastructure.Messaging.Consumers;
using CqrsShowCase.Infrastructure.Messaging.Config;
using Xunit.Abstractions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PocKafka.Tests.Integration;

public class KafkaTestServerFixture : IAsyncLifetime, IDisposable
{
    public ServiceProvider ServiceProvider { get; private set; }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }

    public async Task InitializeAsync()
    {
        try
        {
            var services = new ServiceCollection();
            string basePath = AppContext.BaseDirectory;
            ProducerConfig configProducer = await ConfigFiles.LoadConfig<ProducerConfig>("Infrastructure\\Messaging\\Config\\kafkaproducer.config");
            ConsumerConfig configConsumer = await ConfigFiles.LoadConfig<ConsumerConfig>("Infrastructure\\Messaging\\Config\\kafkaconsumer.config");
            if (configProducer == null || configConsumer == null)
            {
                throw new Exception($"An error occurred while reading the Kafka configuration files.");
            }
            services.AddSingleton<ProducerConfig>(configProducer);
            services.AddSingleton<ConsumerConfig>(configConsumer);
            services.AddScoped<EventProducer>();

            ServiceProvider = services.BuildServiceProvider();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while initializing KafkaTestServerFixture: {ex.Message}");
            throw;
        }
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}
