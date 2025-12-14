using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var competingConsumersQueueName = "CompetingConsumers";
var producer = builder.AddProject<CompetingConsumers_Producer>("CompetingConsumers-Producer")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer1")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(producer);
builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer2")
    .WithEnvironment("QUEUE_NAME", competingConsumersQueueName)
    .WithReference(broker)
    .WaitFor(producer);

var app = builder.Build();

await app.RunAsync();