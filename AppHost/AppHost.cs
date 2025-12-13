using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var producer = builder.AddProject<Producer>("Producer")
    .WithReference(broker)
    .WaitFor(broker);

var _ = builder.AddProject<Consumer>("Consumer")
    .WithReference(broker)
    .WaitFor(producer);

var app = builder.Build();

await app.RunAsync();