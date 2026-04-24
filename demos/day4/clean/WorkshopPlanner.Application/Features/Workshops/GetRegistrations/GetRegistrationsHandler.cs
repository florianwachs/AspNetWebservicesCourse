using MediatR;
using WorkshopPlanner.Application.Abstractions;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.GetRegistrations;

public sealed class GetRegistrationsHandler(IWorkshopRepository repository)
    : IRequestHandler<GetRegistrationsQuery, Result<IReadOnlyList<WorkshopRegistrationResponse>>>
{
    public async Task<Result<IReadOnlyList<WorkshopRegistrationResponse>>> Handle(GetRegistrationsQuery query, CancellationToken ct)
    {
        var workshop = await repository.GetByIdAsync(query.WorkshopId, ct);
        if (workshop is null)
            return Result<IReadOnlyList<WorkshopRegistrationResponse>>.Failure(ErrorType.NotFound, $"Workshop {query.WorkshopId} was not found.");

        IReadOnlyList<WorkshopRegistrationResponse> registrations = workshop.Registrations
            .OrderBy(registration => registration.Id)
            .Select(registration => new WorkshopRegistrationResponse(
                registration.Id,
                registration.AttendeeName,
                registration.AttendeeEmail,
                registration.Status.ToString()))
            .ToArray();

        return Result<IReadOnlyList<WorkshopRegistrationResponse>>.Success(registrations);
    }
}
