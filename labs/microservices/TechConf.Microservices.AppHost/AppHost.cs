var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("techconf-microservices-postgres")
    .WithPgAdmin();

var eventsDb = postgres.AddDatabase("eventsdb");
var sessionsDb = postgres.AddDatabase("sessionsdb");
var registrationsDb = postgres.AddDatabase("registrationsdb");
var notificationsDb = postgres.AddDatabase("notificationsdb");

var messaging = builder.AddRabbitMQ("messaging")
    .WithDataVolume("techconf-microservices-rabbitmq")
    .WithManagementPlugin();

var events = builder.AddProject<Projects.TechConf_Microservices_Events>("events")
    .WithReference(eventsDb)
    .WaitFor(eventsDb)
    .WithHttpHealthCheck("/health");

var sessions = builder.AddProject<Projects.TechConf_Microservices_Sessions>("sessions")
    .WithReference(sessionsDb)
    .WaitFor(sessionsDb)
    .WithHttpHealthCheck("/health");

var registrations = builder.AddProject<Projects.TechConf_Microservices_Registrations>("registrations")
    .WithReference(registrationsDb)
    .WithReference(messaging)
    .WaitFor(registrationsDb)
    .WaitFor(messaging)
    .WithHttpHealthCheck("/health");

var notifications = builder.AddProject<Projects.TechConf_Microservices_Notifications>("notifications")
    .WithReference(notificationsDb)
    .WithReference(messaging)
    .WaitFor(notificationsDb)
    .WaitFor(messaging)
    .WithHttpHealthCheck("/health");

var recommendations = builder.AddNodeApp("recommendations", "../TechConf.Microservices.Recommendations", "server.js")
    .WithHttpEndpoint(env: "PORT")
    .WithHttpHealthCheck("/health");

var gateway = builder.AddProject<Projects.TechConf_Microservices_Gateway>("gateway")
    .WithReference(events)
    .WithReference(sessions)
    .WithReference(registrations)
    .WithReference(notifications)
    .WithReference(recommendations)
    .WaitFor(events)
    .WaitFor(sessions)
    .WaitFor(registrations)
    .WaitFor(notifications)
    .WaitFor(recommendations)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.AddViteApp("web", "../TechConf.Microservices.Web")
    .WithReference(gateway)
    .WaitFor(gateway);

builder.Build().Run();
