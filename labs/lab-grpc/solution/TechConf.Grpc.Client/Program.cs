using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using TechConf.Grpc.Server;

Console.WriteLine("TechConf gRPC Client");
Console.WriteLine("====================");
Console.WriteLine();

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new EventService.EventServiceClient(channel);

// List all events
Console.WriteLine("📋 All Events:");
var response = await client.GetEventsAsync(new GetEventsRequest
{
    Page = 1,
    PageSize = 20
});

foreach (var ev in response.Events)
    Console.WriteLine($"  [{ev.Status}] {ev.Title} — {ev.Location}");
Console.WriteLine($"  Total: {response.TotalCount}");
Console.WriteLine();

// Create a new event
Console.WriteLine("➕ Creating new event...");
var created = await client.CreateEventAsync(new CreateEventRequest
{
    Title = "AI Summit 2026",
    Description = "Exploring the future of AI",
    StartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2026, 12, 1, 9, 0, 0), DateTimeKind.Utc)),
    EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2026, 12, 3, 17, 0, 0), DateTimeKind.Utc)),
    Location = "Hamburg",
    MaxAttendees = 400
});
Console.WriteLine($"  Created: {created.Title} (ID: {created.Id})");
Console.WriteLine();

// Verify it appears
Console.WriteLine("📋 Updated Events:");
var updated = await client.GetEventsAsync(new GetEventsRequest { Page = 1, PageSize = 20 });
foreach (var ev in updated.Events)
    Console.WriteLine($"  [{ev.Status}] {ev.Title} — {ev.Location}");
Console.WriteLine($"  Total: {updated.TotalCount}");

Console.WriteLine();
Console.WriteLine("✅ Client test complete!");
