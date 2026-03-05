using CqrsShowCase.Core.Consumers;

namespace CqrsShowCase.UserInterface.QueryApi;

public class ConsumerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _topic;

    public ConsumerHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
                 ?? configuration["Kafka:Topic"]
                 ?? "SocialMediaPostEvents";
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            using var scope = _scopeFactory.CreateScope();
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            eventConsumer.Consume(_topic, stoppingToken);
        }, stoppingToken);
    }
}
