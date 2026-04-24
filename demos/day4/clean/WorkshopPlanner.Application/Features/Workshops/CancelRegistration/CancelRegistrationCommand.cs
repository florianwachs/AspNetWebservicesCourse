using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CancelRegistration;

public sealed record CancelRegistrationCommand(int WorkshopId, int RegistrationId)
    : IRequest<Result<RegistrationCancelledResponse>>;
