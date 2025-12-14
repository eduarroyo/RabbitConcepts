using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var producer = builder.AddProject<CompetingConsumers_Producer>("CompetingConsumers-Producer")
    .WithReference(broker)
    .WaitFor(broker);

builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer1")
    .WithReference(broker)
    .WaitFor(producer);
builder.AddProject<CompetingConsumers_Consumer>("CompetingConsumers-Consumer2")
    .WithReference(broker)
    .WaitFor(producer);

var app = builder.Build();

await app.RunAsync();