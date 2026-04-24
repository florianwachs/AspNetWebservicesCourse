using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Workshops.GetWorkshopById;

public sealed class GetWorkshopByIdHandler(IWorkshopRepository repository)
{
    public async Task<Result<WorkshopDetailResponse>> Handle(GetWorkshopByIdQuery query, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(query.WorkshopId, ct);
        return workshop is null
            ? Result<WorkshopDetailResponse>.Failure(ErrorType.NotFound, $"Workshop {query.WorkshopId} was not found.")
            : Result<WorkshopDetailResponse>.Success(workshop.ToDetail());
    }
}
