using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var producerConsumerQueueName = "ProducerConsumer";

var producer = builder.AddProject<ProducerConsumer_Producer>("ProducerConsumer-Producer")
    .WithEnvironment("QUEUE_NAME", producerConsumerQueueName)
    .WithReference(broker)
    .WaitFor(broker);

_ = builder.AddProject<ProducerConsumer_Consumer>("ProducerConsumer-Consumer")
    .WithEnvironment("QUEUE_NAME", producerConsumerQueueName)
    .WithReference(broker)
    .WaitFor(producer);

var app = builder.Build();

await app.RunAsync();