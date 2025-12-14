using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

#region Producer-Consumer

var producerConsumerQueueName = "ProducerConsumer";
var producer = builder.AddProject<ProducerConsumer_Producer>("ProducerConsumer-Producer")
    .WithEnvironment("QUEUE_NAME", producerConsumerQueueName)
    .WithReference(broker)
    .WaitFor(broker);

_ = builder.AddProject<ProducerConsumer_Consumer>("ProducerConsumer-Consumer")
    .WithEnvironment("QUEUE_NAME", producerConsumerQueueName)
    .WithReference(broker)
    .WaitFor(producer);

#endregion

#region Competing consumers

var competingConsumersQueueName = "CompetingConsumers";
var producer2 = builder.AddProject<CompetingConsumers_Producer>("CompetingConsumers-Producer")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer1")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(producer2);
builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer2")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(producer2);

#endregion

var app = builder.Build();

await app.RunAsync();