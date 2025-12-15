using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var routingExchhangeName = "Routing";
var publisher = builder.AddProject<Routing_Publisher>("Routing-Publisher")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithReference(broker)
    .WaitFor(broker);

var subscriber = builder.AddProject<Routing_Subscriber>("Routing-Subscriber")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithReference(broker)
    .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();