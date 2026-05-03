using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using TechConf.Grpc.Server;

Console.WriteLine("TechConf gRPC Event Producer");
Console.WriteLine("============================");
Console.WriteLine("Press Ctrl+C to stop publishing events.");
Console.WriteLine();

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

var serverAddress = ResolveServerAddress();
using var channel = GrpcChannel.ForAddress(serverAddress);
var client = new EventService.EventServiceClient(channel);
var random = new Random();
var counter = 1;
string[] locationPool =
[
    "Berlin",
    "Munich",
    "Hamburg",
    "Vienna",
    "Zurich",
    "Prague"
];

Console.WriteLine($"Publishing to {serverAddress}");
Console.WriteLine();

try
{
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        var now = DateTime.UtcNow;
        var startDate = now.AddDays(random.Next(7, 120)).AddHours(random.Next(8, 18) - now.Hour);
        var request = new CreateEventRequest
        {
            Title = $"Live Tech Event #{counter:000}",
            Description = $"Automatically generated event batch item #{counter:000} from the gRPC producer client.",
            StartDate = Timestamp.FromDateTime(startDate),
            EndDate = Timestamp.FromDateTime(startDate.AddHours(8)),
            Location = PickRandom(locationPool, random),
            MaxAttendees = random.Next(120, 751)
        };

        var createdEvent = await client.CreateEventAsync(request, cancellationToken: cancellationTokenSource.Token);
        Console.WriteLine(
            "[{0:HH:mm:ss}] Created {1} in {2} ({3})",
            DateTimeOffset.Now,
            createdEvent.Title,
            createdEvent.Location,
            createdEvent.Id);

        counter++;
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationTokenSource.Token);
    }
}
catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
{
    Console.WriteLine();
    Console.WriteLine("Producer stopped.");
}

static string ResolveServerAddress() =>
    FirstNonEmpty(
        "TECHCONF_GRPC_SERVER_URL",
        "services__api__https__0",
        "services__api__http__0",
        "SERVICES__API__HTTPS__0",
        "SERVICES__API__HTTP__0")
    ?? "https://localhost:5001";

static string? FirstNonEmpty(params string[] keys)
{
    foreach (var key in keys)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
    }

    return null;
}

static string PickRandom(IReadOnlyList<string> items, Random random) =>
    items[random.Next(items.Count)];
