namespace PublisherSubscriber.Subscriber;

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
        var exchangeName = configuration.GetValue<string>("EXCHANGE_NAME")!;
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        CreateChannelOptions channelOptions = new(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, false, true,
            cancellationToken: stoppingToken);
        var queueName = (await channel.QueueDeclareAsync(cancellationToken: stoppingToken)).QueueName;
        var consumer = new AsyncEventingBasicConsumer(channel);
        await channel.QueueBindAsync(queueName, exchangeName, string.Empty, cancellationToken: stoppingToken);

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