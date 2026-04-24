using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;

namespace WorkshopPlanner.Application.Features.Workshops.GetWorkshops;

public sealed class GetWorkshopsHandler(IWorkshopRepository repository)
    : IRequestHandler<GetWorkshopsQuery, IReadOnlyList<WorkshopSummaryResponse>>
{
    public async Task<IReadOnlyList<WorkshopSummaryResponse>> Handle(GetWorkshopsQuery request, CancellationToken ct)
    {
        var workshops = await repository.GetAllAsync(ct);
        return workshops
            .OrderBy(workshop => workshop.Title)
            .Select(workshop => workshop.ToSummary())
            .ToArray();
    }
}
