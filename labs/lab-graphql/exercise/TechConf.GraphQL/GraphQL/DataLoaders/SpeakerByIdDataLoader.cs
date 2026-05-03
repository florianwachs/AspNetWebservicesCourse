using Microsoft.EntityFrameworkCore;
using TechConf.GraphQL.Data;
using TechConf.GraphQL.Models;

namespace TechConf.GraphQL.GraphQL.DataLoaders;

// TODO: Task 5 — Implement SpeakerByIdDataLoader
// This DataLoader solves the N+1 problem when resolving speakers for sessions.
//
// 1. Inherit from BatchDataLoader<Guid, Speaker>
// 2. Inject IDbContextFactory<AppDbContext> (not AppDbContext directly — DataLoaders outlive a single scope)
// 3. Override LoadBatchAsync:
//    - Create a DbContext from the factory
//    - Query speakers WHERE Id IN (keys)
//    - Return as IReadOnlyDictionary<Guid, Speaker>
//
// After implementing the DataLoader, create a type extension for Session:
//
// [ExtendObjectType(typeof(Session))]
// public class SessionResolvers
// {
//     public async Task<Speaker> GetSpeaker(
//         [Parent] Session session,
//         SpeakerByIdDataLoader loader,
//         CancellationToken ct)
//         => await loader.LoadAsync(session.SpeakerId, ct);
// }
//
// Don't forget to register the type extension in Program.cs:
//   .AddTypeExtension<SessionResolvers>()

public class SpeakerByIdDataLoader
{
    // Replace this class with a proper BatchDataLoader<Guid, Speaker> implementation
}
