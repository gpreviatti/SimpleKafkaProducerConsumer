using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infra.Services;
public class ProducerService
{
    private readonly ILogger<ProducerService> _logger;
    private readonly IConfiguration _configuration;

    private readonly IProducer<Null, string> _producer;

    public ProducerService(ILogger<ProducerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        ProducerConfig producerconfig = new()
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        var kafkamessage = new Message<Null, string> { Value = message, };

        await _producer.ProduceAsync(topic, kafkamessage);

        _logger.LogInformation($"{DateTime.Now} | Message sent : {message} to Topic: {topic} ");
    }
}
