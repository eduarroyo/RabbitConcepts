using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var broker = builder.AddRabbitMQ("Broker")
    .WithManagementPlugin();


var requestReplyServer = builder.AddProject<RequestReply_Server>("RequestReply-Server")
    .WithReference(broker)
    .WaitFor(broker);

for (var i = 1; i <= 5; i++)
    builder.AddProject<RequestReply_Client>($"RequestReply-Client-{i}")
        .WithEnvironment("CLIENT_NUMBER", i.ToString())
        .WithReference(broker)
        .WaitFor(requestReplyServer)
        .WaitFor(broker);

var app = builder.Build();
await app.RunAsync();