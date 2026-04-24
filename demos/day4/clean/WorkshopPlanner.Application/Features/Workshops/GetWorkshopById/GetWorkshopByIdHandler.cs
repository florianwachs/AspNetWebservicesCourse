using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.GetWorkshopById;

public sealed class GetWorkshopByIdHandler(IWorkshopRepository repository)
    : IRequestHandler<GetWorkshopByIdQuery, Result<WorkshopDetailResponse>>
{
    public async Task<Result<WorkshopDetailResponse>> Handle(GetWorkshopByIdQuery query, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(query.WorkshopId, ct);
        return workshop is null
            ? Result<WorkshopDetailResponse>.Failure(ErrorType.NotFound, $"Workshop {query.WorkshopId} was not found.")
            : Result<WorkshopDetailResponse>.Success(workshop.ToDetail());
    }
}
