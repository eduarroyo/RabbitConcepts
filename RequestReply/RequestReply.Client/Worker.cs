namespace RequestReply.Client;

public class Worker(
    IConnectionFactory connectionFactory,
    IConfiguration configuration,
    ILogger<Worker> logger
)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var clientNumber = configuration.GetValue<int>("CLIENT_NUMBER");

        var replyQueue = await channel.QueueDeclareAsync($"replyQueue{clientNumber}", exclusive: true,
            cancellationToken: stoppingToken);
        var requestQueue = await channel.QueueDeclareAsync("request-queue", exclusive: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation("Reply received: {message}", message);
            await Task.Yield();
        };
        await channel.BasicConsumeAsync(replyQueue.QueueName, true, consumer,
            stoppingToken);

        const string firstMessage = "Can I request a reply";
        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(firstMessage);


        BasicProperties properties = new()
        {
            ReplyTo = replyQueue.QueueName,
            CorrelationId = Guid.NewGuid().ToString()
        };

        await channel.BasicPublishAsync
        (
            "",
            requestQueue.QueueName,
            true,
            properties,
            body,
            stoppingToken
        );

        logger.LogInformation("Request sent: {correlationId}", properties.CorrelationId);
        logger.LogInformation("Client running");

        Console.ReadLine();
    }
}