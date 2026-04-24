using WorkshopPlanner.Api.Features.Workshops.AddSession;
using WorkshopPlanner.Api.Features.Workshops.CreateWorkshop;
using WorkshopPlanner.Api.Features.Workshops.GetWorkshopById;
using WorkshopPlanner.Api.Features.Workshops.GetWorkshops;
using WorkshopPlanner.Api.Features.Workshops.PublishWorkshop;

namespace WorkshopPlanner.Api.Features.Workshops;

public static class WorkshopEndpoints
{
    public static void MapWorkshopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workshops").WithTags("Workshops");

        GetWorkshopsEndpoint.Map(group);
        GetWorkshopByIdEndpoint.Map(group);
        CreateWorkshopEndpoint.Map(group);
        AddSessionEndpoint.Map(group);
        PublishWorkshopEndpoint.Map(group);
    }
}
