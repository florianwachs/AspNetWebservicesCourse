using System.Globalization;

namespace PartyPlanner;

public class PartyPlanningService
{
    public GuestProfile CreateGuest(GuestRegistration registration, PartyPlan partyPlan)
    {
        var guestName = string.IsNullOrWhiteSpace(registration.Name)
            ? "Mystery Guest"
            : registration.Name;

        var theme = NormalizeTheme(registration.FavoriteTheme, partyPlan.AvailableThemes);

        // int.TryParse keeps bad console input from crashing the app.
        var snackCount = int.TryParse(registration.SnackInput, out var parsedSnackCount) && parsedSnackCount >= 0
            ? parsedSnackCount
            : 0;

        return new GuestProfile(
            Name: guestName,
            FavoriteTheme: theme,
            SnackPacks: snackCount,
            ArrivalLabel: ToArrivalLabel(snackCount));
    }

    public GuestProfile CreateHostGuest(string theme) =>
        new("Workshop Host", theme, 2, "keeps the playlist alive");

    public async Task<PartyResult> BuildResultAsync(
        PartyPlan partyPlan,
        string selectedTheme,
        IReadOnlyCollection<GuestProfile> guests,
        CancellationToken cancellationToken)
    {
        // In a real app this could be a database or HTTP call.
        await Task.Delay(250, cancellationToken);

        var totalSnacks = guests.Sum(guest => guest.SnackPacks);

        var energyLevel = totalSnacks switch
        {
            0 => "Emergency pizza mode",
            <= 3 => "Respectable snack effort",
            <= 6 => "Workshop ready",
            _ => "Party hero"
        };

        var summary =
            $"{partyPlan.Title} is scheduled for {partyPlan.StartsOn.ToString("D", CultureInfo.InvariantCulture)}. " +
            $"{guests.Count} guest(s) are bringing {totalSnacks} snack pack(s).";

        return new PartyResult(
            Summary: summary,
            EnergyLevel: energyLevel,
            Motto: PickMotto(selectedTheme, totalSnacks, partyPlan.AvailableThemes),
            GeneratedAt: DateTimeOffset.UtcNow);
    }

    private static string NormalizeTheme(string? themeInput, IReadOnlyList<string> availableThemes) =>
        availableThemes.FirstOrDefault(theme =>
            theme.Equals(themeInput, StringComparison.OrdinalIgnoreCase))
        ?? availableThemes[0];

    private static string ToArrivalLabel(int snackCount) => snackCount switch
    {
        <= 0 => "just vibes",
        <= 2 => "light snack energy",
        <= 5 => "solid teammate",
        _ => "legend status"
    };

    private static string PickMotto(
        string selectedTheme,
        int totalSnacks,
        IReadOnlyList<string> availableThemes)
    {
        var mottoLookup = new Dictionary<string, string[]>
        {
            ["Retro games"] =
            [
                "Pause. Debug. Snack.",
                "Eight bits, zero panic."
            ],
            ["Space tacos"] =
            [
                "Launch code: salsa.",
                "One small snack for devs, one giant leap for lunch."
            ],
            ["Robots in hoodies"] =
            [
                "Automate the boring parts, enjoy the snacks.",
                "Clean code. Warm hoodies. Happy bots."
            ]
        };

        var normalizedTheme = NormalizeTheme(selectedTheme, availableThemes);
        var mottoOptions = mottoLookup[normalizedTheme];
        var index = totalSnacks % mottoOptions.Length;

        return mottoOptions[index];
    }
}
