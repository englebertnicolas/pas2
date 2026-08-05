using PAS.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var dbCnc = builder.AddConnectionString("Database");

var dbMigrator = builder.AddProject<Projects.PAS_DbMigrator>("dbmigrator")
    .WithReference(dbCnc);

var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);//.WithDataVolume();

builder.AddProject<Projects.PAS_Assets_Api>("api-assets")
    .WithReference(dbCnc)
    .WithReference(rabbitMq)
    .WithScalarEndpoint()
    .WaitForCompletion(dbMigrator)
    .WaitFor(rabbitMq);

builder.AddProject<Projects.PAS_Policies_Api>("api-policies")
    .WithReference(dbCnc)
    .WithReference(rabbitMq)
    .WithScalarEndpoint()
    .WaitForCompletion(dbMigrator)
    .WaitFor(rabbitMq);

builder.AddProject<Projects.PAS_ActuarialEngine_Api>("api-actuarialengine")
    .WithReference(dbCnc)
    .WithReference(rabbitMq)
    .WithScalarEndpoint()
    .WaitForCompletion(dbMigrator)
    .WaitFor(rabbitMq);

builder.Build().Run();
