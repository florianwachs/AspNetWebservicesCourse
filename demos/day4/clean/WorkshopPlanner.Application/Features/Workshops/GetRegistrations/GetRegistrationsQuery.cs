using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.GetRegistrations;

public sealed record GetRegistrationsQuery(int WorkshopId) : IRequest<Result<IReadOnlyList<WorkshopRegistrationResponse>>>;
