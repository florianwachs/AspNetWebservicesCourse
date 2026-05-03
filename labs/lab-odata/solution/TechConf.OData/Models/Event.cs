namespace TechConf.OData.Models;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxAttendees { get; set; }
    public string Status { get; set; } = "Draft";
    public ICollection<Session> Sessions { get; set; } = [];
    public ICollection<Registration> Registrations { get; set; } = [];
}
