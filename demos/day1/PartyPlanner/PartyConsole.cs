using System.Globalization;

namespace PartyPlanner;

public class PartyConsole
{
    public void ShowHeader(PartyPlan partyPlan)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(partyPlan.Title);
        Console.ResetColor();
        Console.WriteLine($"Party date: {partyPlan.StartsOn.ToString("D", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Playlist: {partyPlan.PlaylistUrl}");
        Console.WriteLine();
    }

    public GuestRegistration ReadGuestRegistration(IReadOnlyList<string> availableThemes)
    {
        Console.Write("What's your name? ");
        var name = Console.ReadLine()?.Trim();

        Console.Write($"Pick a theme ({string.Join(" / ", availableThemes)}): ");
        var theme = Console.ReadLine()?.Trim();

        Console.Write("How many snack packs will you bring? ");
        var snackInput = Console.ReadLine();

        return new GuestRegistration(name, theme, snackInput);
    }

    public void ShowGuestList(IEnumerable<GuestProfile> guests)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Guest list");
        Console.ResetColor();

        foreach (var guest in guests.OrderByDescending(guest => guest.SnackPacks))
        {
            Console.WriteLine(
                $"- {guest.Name,-14} | {guest.FavoriteTheme,-17} | {guest.SnackPacks,2} snack pack(s) | {guest.ArrivalLabel}");
        }
    }

    public void ShowResult(string selectedTheme, PartyResult result)
    {
        Console.WriteLine();
        Console.WriteLine($"Theme selected: {selectedTheme}");
        Console.WriteLine(result.Summary);
        Console.WriteLine($"Energy level: {result.EnergyLevel}");
        Console.WriteLine($"Party motto: {result.Motto}");
        Console.WriteLine($"Generated at (UTC): {result.GeneratedAt:O}");
    }
}
