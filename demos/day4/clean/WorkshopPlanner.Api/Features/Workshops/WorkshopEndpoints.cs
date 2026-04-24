using WorkshopPlanner.Api.Features.Workshops.AddSession;
using WorkshopPlanner.Api.Features.Workshops.CancelRegistration;
using WorkshopPlanner.Api.Features.Workshops.CancelWorkshop;
using WorkshopPlanner.Api.Features.Workshops.CheckInRegistration;
using WorkshopPlanner.Api.Features.Workshops.CreateRegistration;
using WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;
using WorkshopPlanner.Api.Features.Workshops.GetRegistrations;
using WorkshopPlanner.Api.Features.Workshops.GetSessions;
using WorkshopPlanner.Api.Features.Workshops.GetWorkshopById;
using WorkshopPlanner.Api.Features.Workshops.GetWorkshops;
using WorkshopPlanner.Api.Features.Workshops.PublishWorkshop;
using WorkshopPlanner.Api.Features.Workshops.RemoveSession;
using WorkshopPlanner.Api.Features.Workshops.UpdateSession;
using WorkshopPlanner.Api.Features.Workshops.UpdateWorkshop;

namespace WorkshopPlanner.Api.Features.Workshops;

public static class WorkshopEndpoints
{
    public static void MapWorkshopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workshops").WithTags("Workshops");

        GetWorkshopsEndpoint.Map(group);
        GetWorkshopByIdEndpoint.Map(group);
        CreateWorkshopEndpoint.Map(group);
        UpdateWorkshopEndpoint.Map(group);
        CancelWorkshopEndpoint.Map(group);
        GetSessionsEndpoint.Map(group);
        AddSessionEndpoint.Map(group);
        UpdateSessionEndpoint.Map(group);
        RemoveSessionEndpoint.Map(group);
        PublishWorkshopEndpoint.Map(group);
        GetRegistrationsEndpoint.Map(group);
        CreateRegistrationEndpoint.Map(group);
        CancelRegistrationEndpoint.Map(group);
        CheckInRegistrationEndpoint.Map(group);
    }
}
