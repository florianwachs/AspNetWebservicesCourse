using WorkshopPlanner.Application.Abstractions;

namespace WorkshopPlanner.Application.Workshops.GetWorkshops;

public sealed class GetWorkshopsHandler(IWorkshopRepository repository)
{
    public async Task<IReadOnlyList<WorkshopSummaryResponse>> Handle(CancellationToken ct)
    {
        var workshops = await repository.GetAllAsync(ct);
        return workshops
            .OrderBy(workshop => workshop.Title)
            .Select(workshop => workshop.ToSummary())
            .ToArray();
    }
}
