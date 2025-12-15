using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var paymentsBindingKey = "payments";
var analyticsBindingKey = "analytics";

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var routingExchhangeName = "Routing";
var publisher = builder.AddProject<Routing_Publisher>("Routing-Publisher")
    .WithEnvironment("BINDING_KEYS", $"{paymentsBindingKey},{analyticsBindingKey}")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithReference(broker)
    .WaitFor(broker);

var analyticsSubscriber = builder.AddProject<Routing_Subscriber>("Routing-Subscriber-Analytics")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithEnvironment("BINDING_KEY", analyticsBindingKey)
    .WithReference(broker)
    .WaitFor(broker);

var paymentsSubscriber = builder.AddProject<Routing_Subscriber>("Routing-Subscriber-Payments")
    .WithEnvironment("EXCHANGE_NAME", routingExchhangeName)
    .WithEnvironment("BINDING_KEY", paymentsBindingKey)
    .WithReference(broker)
    .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();