namespace Consumer;

public class Worker(
    ILogger<Worker> logger,
    IConnectionFactory connectionFactory,
    IConfiguration configuration
)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting  at: {time}", DateTimeOffset.Now);
        var queueName = configuration.GetValue<string>("QUEUE_NAME")!;
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        CreateChannelOptions channelOptions = new(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        _ = channel.QueueDeclareAsync(queueName, false, false, false, cancellationToken: stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation("Received message: {message}", message);
            await Task.Yield();
        };

        await channel.BasicConsumeAsync(queueName, true, consumer, stoppingToken);
        logger.LogInformation("Stopping at: {time}", DateTimeOffset.Now);
    }
}