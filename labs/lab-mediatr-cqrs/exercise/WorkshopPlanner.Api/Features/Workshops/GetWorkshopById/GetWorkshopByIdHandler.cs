using MediatR;
using WorkshopPlanner.Api.Common;
using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Features.Workshops.Shared;

namespace WorkshopPlanner.Api.Features.Workshops.GetWorkshopById;

public sealed class GetWorkshopByIdHandler(WorkshopStore store)
    : IRequestHandler<GetWorkshopByIdQuery, Result<WorkshopDetailResponse>>
{
    public Task<Result<WorkshopDetailResponse>> Handle(GetWorkshopByIdQuery request, CancellationToken ct)
    {
        var workshop = store.Workshops.FirstOrDefault(item => item.Id == request.WorkshopId);
        return Task.FromResult(
            workshop is null
                ? Result<WorkshopDetailResponse>.Failure(ErrorType.NotFound, $"Workshop {request.WorkshopId} was not found.")
                : Result<WorkshopDetailResponse>.Success(workshop.ToDetail()));
    }
}
