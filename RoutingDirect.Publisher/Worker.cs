namespace RoutingDirect.Publisher;

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
        var bindingKeys = configuration.GetValue<string>("BINDING_KEYS")!.Split(",");
        var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        CreateChannelOptions channelOptions = new(false, false);
        var channel = await connection.CreateChannelAsync(channelOptions, stoppingToken);
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, cancellationToken: stoppingToken);

        var messageCounter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var bindingKey = bindingKeys[++messageCounter % 3];
            var message = $"Message {messageCounter} - {bindingKey}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchangeName, bindingKey, encodedMessage, stoppingToken);
            logger.LogInformation("Published message: {message}", message);
            await Task.Delay(1000, stoppingToken);
        }

        logger.LogInformation("Stopping at: {time}", DateTimeOffset.Now);
    }
}