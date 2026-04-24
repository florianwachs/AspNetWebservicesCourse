using MediatR;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshops;

public sealed class GetWorkshopsHandler(WorkshopStore store)
    : IRequestHandler<GetWorkshopsQuery, IReadOnlyList<WorkshopSummaryResponse>>
{
    public Task<IReadOnlyList<WorkshopSummaryResponse>> Handle(GetWorkshopsQuery request, CancellationToken ct)
    {
        IReadOnlyList<WorkshopSummaryResponse> workshops = store.Workshops
            .OrderBy(workshop => workshop.Title)
            .Select(workshop => workshop.ToSummary())
            .ToArray();

        return Task.FromResult(workshops);
    }
}
