using MediatR;
using WorkshopPlanner.Application.Features.Workshops.Shared;
using WorkshopPlanner.Domain.Common;

namespace WorkshopPlanner.Application.Features.Workshops.CreateRegistration;

public sealed record CreateRegistrationCommand(int WorkshopId, string AttendeeName, string AttendeeEmail)
    : IRequest<Result<RegistrationCreatedResponse>>;
