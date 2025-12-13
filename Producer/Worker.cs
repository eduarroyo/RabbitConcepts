namespace Producer;

public class Worker(ILogger<Worker> logger, IConnectionFactory connectionFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting  at: {time}", DateTimeOffset.Now);
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        var channelOptions = new CreateChannelOptions(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        _ = channel.QueueDeclareAsync("letterbox", false, false, false, cancellationToken: stoppingToken);

        var messageCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = $"Message {++messageCounter}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync("", "letterbox", encodedMessage, stoppingToken);
            logger.LogInformation("Published message: {message}", message);
            await Task.Delay(1000, stoppingToken);
        }

        logger.LogInformation("Stopping at: {time}", DateTimeOffset.Now);
    }
}