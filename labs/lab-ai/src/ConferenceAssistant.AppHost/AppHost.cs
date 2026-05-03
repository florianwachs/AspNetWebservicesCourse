var builder = DistributedApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Azure OpenAI — reference an existing resource via user secrets
// Required user secrets (set in AppHost project):
//   dotnet user-secrets set "Azure:SubscriptionId" "<subscription-id>"
//   dotnet user-secrets set "Azure:Location" "eastus"
//   dotnet user-secrets set "AzureOpenAI:Name" "<resource-name>"
//   dotnet user-secrets set "AzureOpenAI:ResourceGroup" "<resource-group>"
// ---------------------------------------------------------------------------
var azOpenAiName = builder.AddParameterFromConfiguration("AzureOpenAIName", "AzureOpenAI:Name");
var azOpenAiRg = builder.AddParameterFromConfiguration("AzureOpenAIResourceGroup", "AzureOpenAI:ResourceGroup");

var openai = builder.AddAzureOpenAI("openai")
    .RunAsExisting(azOpenAiName, azOpenAiRg);

// Deployment names must match what exists in your Azure OpenAI resource
openai.AddDeployment("chat", "gpt-4o", "2024-08-06");
openai.AddDeployment("embedding", "text-embedding-3-small", "1");

// PostgreSQL for EF Core persistence (sessions, polls, Q&A, insights)
var postgres = builder.AddPostgres("postgres")
    .WithPgWeb()
    .WithDataVolume();

var conferenceDb = postgres.AddDatabase("conferencedb");

// Qdrant for vector/embedding storage (semantic search)
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var web = builder.AddProject<Projects.ConferenceAssistant_Web>("web")
    .WithReference(openai)
    .WithReference(conferenceDb)
    .WithReference(qdrant)
    .WaitFor(openai)
    .WaitFor(conferenceDb)
    .WaitFor(qdrant);

// Dev tunnel — exposes the web app via a public HTTPS URL for attendees
builder.AddDevTunnel("conference-tunnel")
    .WithReference(web)
    .WithAnonymousAccess();

builder.Build().Run();
