using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();

var publisher = builder.AddProject<RequestReply_Client>("RequestReply-Client")
    .WithReference(broker)
    .WaitFor(broker);

var requestReplyServer = builder.AddProject<RequestReply_Server>("RequestReply-Server")
    .WithReference(broker)
    .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();