# C# Crash Course for Modern ASP.NET Development

## Why this module exists

This course is about building modern web services with ASP.NET Core.  
To move fast, we align on a practical C# baseline first.

We start with a small **console app** so everyone can get comfortable with the language before we move into APIs.

We focus on:

- **Modern C# style** (top-level statements, records, pattern matching, null safety, async/await)
- **Day 1 CLI habits** (`dotnet new`, `dotnet run`, `dotnet add package`)
- **Core .NET base classes** you'll use in every lab (`HttpClient`, `CancellationToken`, `DateTimeOffset`, `Uri`, etc.)

---

## 1) Start with a tiny console app

Create a new project:

```bash
dotnet new console -n PartyPlanner
cd PartyPlanner
```

That gives you a simple `Program.cs`. In modern C#, console apps can use **top-level statements**, so you can write code directly without first creating a `Program` class and `Main` method.

Replace the file with this:

```csharp
using System.Globalization;

var planner = new PartyPlanner("Code & Snacks Night", DateOnly.FromDateTime(DateTime.Today.AddDays(7)));

Console.Write("What's your name? ");
var guestName = Console.ReadLine()?.Trim();

Console.Write("How many snack packs will you bring? ");
var snackInput = Console.ReadLine();

var snackCount = int.TryParse(snackInput, out var parsedSnackCount)
    ? parsedSnackCount
    : 0;

var guest = new Guest(
    Name: string.IsNullOrWhiteSpace(guestName) ? "Mystery Guest" : guestName,
    SnackPacks: snackCount,
    ArrivalLabel: snackCount switch
    {
        <= 0 => "just vibes",
        <= 2 => "light snack energy",
        _ => "legend status"
    });

Console.WriteLine();
Console.WriteLine($"Welcome to {planner.Title}!");
Console.WriteLine($"{guest.Name} is bringing {guest.SnackPacks} snack pack(s) — {guest.ArrivalLabel}.");
Console.WriteLine($"Party date: {planner.StartsOn.ToString("D", CultureInfo.InvariantCulture)}");

public record PartyPlanner(string Title, DateOnly StartsOn);
public record Guest(string Name, int SnackPacks, string ArrivalLabel);
```

Run it:

```bash
dotnet run
```

### What this sample already teaches

- **Top-level statements** keep small apps easy to read
- **`record` types** are great for small immutable data objects
- **`var`** is fine when the type is obvious from the right-hand side
- **Pattern matching** with `switch` helps express rules clearly
- **`int.TryParse`** is the safe beginner-friendly way to read numeric input

---

## 2) Modern C# in 15 minutes

### Prefer immutable data with `record`

```csharp
public record EventDto(
    int Id,
    string Title,
    DateTimeOffset StartUtc,
    string City);
```

Use records for request/response models or small domain objects when possible:

- Value-based equality
- Concise syntax
- Great fit for serialization

### Nullable reference types are not optional

Treat nullable warnings as design feedback:

```csharp
string name = "Ada";
string? nickname = null;

Console.WriteLine(name.Length);
Console.WriteLine(nickname?.Length ?? 0);
```

If a value may be missing, model it as nullable (`string?`, `DateTimeOffset?`).

### Use pattern matching for readable branching

```csharp
static string ToLabel(int score) => score switch
{
    >= 90 => "Outstanding",
    >= 70 => "Great",
    >= 40 => "Solid start",
    _ => "Keep practicing"
};
```

### Async all the way

Use `async`/`await` for I/O (database, HTTP, file/network calls):

```csharp
using HttpClient client = new();

var html = await client.GetStringAsync("https://example.org");
Console.WriteLine($"Downloaded {html.Length} characters.");
```

---

## 3) NuGet packages: what they are and how to use them

**NuGet** is the package manager for .NET.  
If your app needs functionality that is not in the base class library, you can add a package.

A great beginner-friendly example is [`Spectre.Console`](https://spectreconsole.net/), which makes console output prettier and easier to read.

Install it:

```bash
dotnet add package Spectre.Console
```

That command updates your `.csproj` file with a package reference like this:

```xml
<ItemGroup>
  <PackageReference Include="Spectre.Console" Version="0.54.0" />
</ItemGroup>
```

Then you can use it in code:

```csharp
using Spectre.Console;

AnsiConsole.MarkupLine("[green]Console apps can be fun too![/]");
```

Here is a small upgraded version of the sample:

```csharp
using Spectre.Console;

var theme = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Pick tonight's coding party theme:")
        .AddChoices("Retro games", "Space tacos", "Robots in hoodies"));

AnsiConsole.MarkupLine($"[yellow]Theme selected:[/] {theme}");
```

### When to use a package

Add a package when it gives you real value, for example:

- Better console UI (`Spectre.Console`)
- JSON APIs and serializers beyond the built-in defaults
- Database access libraries like EF Core
- Testing libraries such as xUnit and FluentAssertions

Before adding a package, ask:

1. Do I really need it?
2. Is it well known and maintained?
3. Does .NET already provide what I need?

---

## 4) Core base classes for this course

These are the most important BCL/ASP.NET types you'll repeatedly use.

| Type                                                     | Why it matters in this course                        | Common usage                               |
| -------------------------------------------------------- | ---------------------------------------------------- | ------------------------------------------ |
| `HttpClient`                                             | Calling external APIs safely from your service       | Outbound REST calls, integration scenarios |
| `HttpRequestMessage` / `HttpResponseMessage`             | Full request/response control when needed            | Headers, methods, status inspection        |
| `CancellationToken`                                      | Cooperative cancellation for scalable APIs           | Pass from endpoint to DB/HTTP calls        |
| `DateTimeOffset`                                         | Correct timezone-aware timestamps                    | Event start/end times, auditing            |
| `DateOnly`                                               | Clear date-only values for small apps and scheduling | Birthdays, parties, daily plans            |
| `Uri`                                                    | Safe URL construction and validation                 | Building links, external endpoints         |
| `Task` / `Task<T>`                                       | Async return types                                   | Non-blocking API handlers                  |
| `IEnumerable<T>` / `List<T>` / `Dictionary<TKey,TValue>` | Standard collections                                 | DTO lists, lookups, in-memory stores       |
| `ILogger<T>`                                             | Structured logging for diagnostics                   | App insights, debugging labs               |
| `JsonSerializer` (`System.Text.Json`)                    | High-performance JSON in .NET                        | Serialization/deserialization behavior     |

### `HttpClient` essentials (important later in the course)

`HttpClient` is intended to be reused. In ASP.NET Core, use `IHttpClientFactory` through DI:

```csharp
builder.Services.AddHttpClient("weather", client =>
{
    client.BaseAddress = new Uri("https://api.weather.example/");
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("TechConfApi/1.0");
});
```

Then consume it in an endpoint or service:

```csharp
app.MapGet("/weather", async (IHttpClientFactory factory, CancellationToken ct) =>
{
    var client = factory.CreateClient("weather");
    using var response = await client.GetAsync("forecast", ct);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync(ct);
    return TypedResults.Content(json, "application/json");
});
```

### Always flow `CancellationToken`

Pass `CancellationToken` to async APIs whenever available:

- `ToListAsync(ct)` in EF Core
- `SendAsync(request, ct)` in `HttpClient`
- `Task.Delay(..., ct)` in background work

This prevents wasted work when requests are cancelled.

---

## 5) Idiomatic C# conventions we follow in labs

- Use **PascalCase** for types/methods/properties, **camelCase** for locals/parameters
- Keep methods and endpoint handlers short; move logic into focused services when it grows
- Prefer explicit DTOs over vague `object` values
- Return typed results (`TypedResults.Ok(...)`, `TypedResults.NotFound()`, etc.) once we reach APIs
- Avoid `DateTime.Now` for shared app contracts; prefer `DateTimeOffset.UtcNow`
- Avoid `.Result` / `.Wait()` on tasks (can cause thread starvation/deadlocks)

---

## 6) Mini lab — Build a fun console app

Build your own **Mood-Powered Party Planner**. The goal is to practice C# basics without worrying about HTTP, routing, or ASP.NET yet.

### Lab goal

Create a console app that asks the user a few questions and prints a funny event plan.

Example idea:

- User enters their name
- User picks a party theme
- User enters a snack count
- App prints a playful summary and a "party readiness" score

### Suggested steps

#### Task 1: Scaffold the app

```bash
dotnet new console -n MoodPartyPlanner
cd MoodPartyPlanner
```

Replace `Program.cs` with your own top-level statement version.

#### Task 2: Add a couple of records

Create records such as:

```csharp
public record GuestProfile(string Name, string FavoriteTheme, int SnackPacks);
public record PartyResult(string Summary, string EnergyLevel);
```

#### Task 3: Read input from the console

Use `Console.ReadLine()` and safe parsing with `int.TryParse`.

#### Task 4: Add a fun rule with pattern matching

Examples:

- `0` snacks => `"Emergency pizza mode"`
- `1` or `2` snacks => `"Respectable snack effort"`
- `3+` snacks => `"Party hero"`

#### Task 5: Add one NuGet package

Install `Spectre.Console`:

```bash
dotnet add package Spectre.Console
```

Then make at least one line of output more fun with color or a prompt.

#### Task 6: Make it your own

Add one custom twist:

- a random party motto
- emoji output
- a "chaos level" score
- a second guest
- a bonus theme chooser

### Stretch goals

1. Store several guests in a `List<GuestProfile>` and print them in a loop.
2. Sort guests by snack count.
3. Save a short summary to a text file.
4. Fetch a random joke or quote with `HttpClient`.

The important part is not building a perfect app. The important part is practicing:

- C# syntax
- reading input
- records
- control flow
- packages
- running and re-running code from the CLI

---

## 7) Quick self-check before continuing Day 1

You are ready for the next modules if you can answer:

1. What problem do top-level statements solve in a console app?
2. When should I use `record` instead of a mutable class?
3. Why is `int.TryParse` safer than `int.Parse` for user input?
4. What does `dotnet add package` do?
5. Why should `HttpClient` come from DI/factory instead of `new` per request in ASP.NET apps?

> [!NOTE]
> I am here to help you. There are no stupid questions and we all come from different backgrounds and experiences.
