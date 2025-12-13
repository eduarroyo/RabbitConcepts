using System.Text;
using RabbitMQ.Client;

namespace Producer;

public class Worker(ILogger<Worker> logger, IConnectionFactory connectionFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        var channelOptions = new CreateChannelOptions(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        var queue = channel.QueueDeclareAsync("letterbox", false, false, false,
            null, cancellationToken: stoppingToken);

        logger.LogInformation("Starting  at: {time}", DateTimeOffset.Now);
        var messageCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = $"Mensaje {++messageCounter}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync("", "letterbox", encodedMessage, stoppingToken);
            logger.LogInformation("Published message: {message}", message);
            await Task.Delay(1000, stoppingToken);
        }

        logger.LogInformation("Stopping at: {time}", DateTimeOffset.Now);
    }
}