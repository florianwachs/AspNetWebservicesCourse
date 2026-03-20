namespace PartyPlanner;

public class PartyPlannerApp(PartyConsole partyConsole, PartyPlanningService planningService)
{
    public async Task RunAsync(PartyPlan partyPlan, CancellationToken cancellationToken = default)
    {
        if (partyPlan.AvailableThemes.Count == 0)
        {
            throw new InvalidOperationException("The party plan needs at least one theme.");
        }

        partyConsole.ShowHeader(partyPlan);

        var registration = partyConsole.ReadGuestRegistration(partyPlan.AvailableThemes);
        var primaryGuest = planningService.CreateGuest(registration, partyPlan);

        var guests = new List<GuestProfile>
        {
            primaryGuest,
            planningService.CreateHostGuest(primaryGuest.FavoriteTheme)
        };

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(2));

        var result = await planningService.BuildResultAsync(
            partyPlan,
            primaryGuest.FavoriteTheme,
            guests,
            timeout.Token);

        partyConsole.ShowGuestList(guests);
        partyConsole.ShowResult(primaryGuest.FavoriteTheme, result);
    }
}
