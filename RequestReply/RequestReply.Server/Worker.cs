namespace RequestReply.Server;

public class Worker(IConnectionFactory connectionFactory, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        var requestQueue = await channel.QueueDeclareAsync("request-queue", exclusive: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            logger.LogInformation("Received message: {correlationId}", ea.BasicProperties.CorrelationId);
            var replyMessage = $"This is your reply {ea.BasicProperties.CorrelationId}";
            var body = Encoding.UTF8.GetBytes(replyMessage);
            await channel.BasicPublishAsync
            (
                "",
                ea.BasicProperties.ReplyTo,
                body,
                stoppingToken
            );
            logger.LogInformation("Reply sent: {correlationId}", ea.BasicProperties.CorrelationId);
        };

        await channel.BasicConsumeAsync(requestQueue.QueueName, true, consumer, stoppingToken);
        logger.LogInformation("Server running");

        Console.ReadLine();
    }
}