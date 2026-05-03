namespace TechConf.Grpc.Server.Models;

public class Speaker
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public ICollection<Session> Sessions { get; set; } = [];
}
