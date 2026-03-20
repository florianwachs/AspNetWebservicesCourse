using PartyPlanner;

var partyPlan = new PartyPlan(
    Title: "Code & Snacks Night",
    StartsOn: DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
    PlaylistUrl: new Uri("https://example.org/playlists/code-and-snacks"),
    AvailableThemes:
    [
        "Retro games",
        "Space tacos",
        "Robots in hoodies"
    ]);

var app = new PartyPlannerApp(
    new PartyConsole(),
    new PartyPlanningService());

await app.RunAsync(partyPlan);
