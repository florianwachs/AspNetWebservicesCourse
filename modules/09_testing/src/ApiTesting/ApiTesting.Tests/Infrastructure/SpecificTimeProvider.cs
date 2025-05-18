namespace ApiTesting.Tests.Infrastructure;

public class SpecificTimeProvider(DateTimeOffset specificTime) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => specificTime;
}