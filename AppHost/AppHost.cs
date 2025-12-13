var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var producer = builder.AddProject<Projects.Producer>("Producer")
    .WaitFor(broker);

var consumer = builder.AddProject<Projects.Consumer>("Consumer")
    .WaitFor(broker)
    .WaitFor(producer);

var app = builder.Build();
    
await app.RunAsync();