using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infra.Services;

public sealed class ConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;

    private readonly ILogger<ConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public ConsumerService(ILogger<ConsumerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        ConsumerConfig consumerConfig = new()
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "group-1",
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true,
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe("my-topic");

        while (!cancellationToken.IsCancellationRequested)
            ProcessKafkaMessage(cancellationToken);

        _consumer.Close();
    }

    private void ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        try
        {
            var consumeResult = _consumer.Consume(stoppingToken);

            var message = consumeResult.Message.Value;

            _logger.LogInformation($"{DateTime.Now} | Message received: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing Kafka message: {ex.Message}");
        }
    }
}
