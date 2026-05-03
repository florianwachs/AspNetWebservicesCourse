namespace TechConf.Api.Models.V2;

public record EventResponse(
    int Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    VenueResponse Venue,
    int RegisteredCount,
    string Status);

public record VenueResponse(string Name, string Address, string City, int Capacity);
