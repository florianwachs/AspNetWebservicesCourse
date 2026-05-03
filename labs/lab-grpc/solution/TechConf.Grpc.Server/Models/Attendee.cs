namespace TechConf.Grpc.Server.Models;

public class Attendee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Registration> Registrations { get; set; } = [];
}
