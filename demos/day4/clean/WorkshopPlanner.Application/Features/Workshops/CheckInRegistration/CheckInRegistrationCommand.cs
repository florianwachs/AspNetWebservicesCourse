using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CheckInRegistration;

public sealed record CheckInRegistrationCommand(int WorkshopId, int RegistrationId)
    : IRequest<Result<RegistrationCheckedInResponse>>;
