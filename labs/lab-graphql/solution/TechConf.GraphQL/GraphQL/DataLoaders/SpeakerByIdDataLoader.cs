using Microsoft.EntityFrameworkCore;
using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL.DataLoaders;

public class SpeakerByIdDataLoader : BatchDataLoader<Guid, Speaker>
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public SpeakerByIdDataLoader(
        IDbContextFactory<AppDbContext> dbFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbFactory = dbFactory;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Speaker>> LoadBatchAsync(
        IReadOnlyList<Guid> keys, CancellationToken ct)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        return await db.Speakers
            .Where(s => keys.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, ct);
    }
}

[ExtendObjectType(typeof(Session))]
public class SessionResolvers
{
    public async Task<Speaker> GetSpeaker(
        [Parent] Session session,
        SpeakerByIdDataLoader loader,
        CancellationToken ct)
        => await loader.LoadAsync(session.SpeakerId, ct);
}
