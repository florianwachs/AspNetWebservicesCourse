using Microsoft.Extensions.Logging;

namespace TechConf.ConsoleDi;

public class OperationMarker
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class WorkshopScope
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class AgendaPrinter(
    TimeProvider clock,
    OperationMarker operationMarker,
    WorkshopScope workshopScope,
    ILogger<AgendaPrinter> logger)
{
    public void Print(string topic)
    {
        var now = clock.GetLocalNow();

        logger.LogInformation(
            "Topic {Topic} -> transient={TransientId} scoped={ScopedId}",
            topic,
            operationMarker.Id,
            workshopScope.Id);

        Console.WriteLine(
            $"{now:HH:mm:ss} | {topic,-22} | transient {operationMarker.Id} | scoped {workshopScope.Id}");
    }
}
