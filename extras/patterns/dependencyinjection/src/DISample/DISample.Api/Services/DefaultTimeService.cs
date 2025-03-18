namespace DISample.Api.Services;

public class DefaultTimeService : ITimeService
{
    public DateTime Now => DateTime.UtcNow;
}
