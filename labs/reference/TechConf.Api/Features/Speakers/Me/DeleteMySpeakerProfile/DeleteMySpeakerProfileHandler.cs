using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TechConf.Api.Data;
using TechConf.Api.Infrastructure.Auth;
using TechConf.Api.Infrastructure.Exceptions;

namespace TechConf.Api.Features.Speakers.Me.DeleteMySpeakerProfile;

public sealed class DeleteMySpeakerProfileHandler(
    AppDbContext db,
    HybridCache cache,
    ICurrentUserAccessor currentUser)
    : IRequestHandler<DeleteMySpeakerProfileCommand>
{
    public async Task Handle(DeleteMySpeakerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new ForbiddenException("You must be signed in.");

        var profile = await db.SpeakerProfiles.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (profile is null)
        {
            throw new NotFoundException("Speaker profile", "me");
        }

        db.SpeakerProfiles.Remove(profile);
        await db.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync("speakers", cancellationToken);
    }
}
