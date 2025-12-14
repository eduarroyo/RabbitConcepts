using RabbitMQ.Client.Events;

namespace Consumer;

public class Worker(ILogger<Worker> logger, IConnectionFactory connectionFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting  at: {time}", DateTimeOffset.Now);
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        var channelOptions = new CreateChannelOptions(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        _ = channel.QueueDeclareAsync("letterbox", false, false, false,
            cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation("Received message: {message}", message);
            await Task.Yield();
        };

        await channel.BasicConsumeAsync("letterbox", true, consumer, stoppingToken);
        logger.LogInformation("Stopping at: {time}", DateTimeOffset.Now);
    }
}