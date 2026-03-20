namespace PartyPlanner;

public record PartyPlan(
    string Title,
    DateOnly StartsOn,
    Uri PlaylistUrl,
    IReadOnlyList<string> AvailableThemes);

public record GuestRegistration(string? Name, string? FavoriteTheme, string? SnackInput);

public record GuestProfile(string Name, string FavoriteTheme, int SnackPacks, string ArrivalLabel);

public record PartyResult(string Summary, string EnergyLevel, string Motto, DateTimeOffset GeneratedAt);
