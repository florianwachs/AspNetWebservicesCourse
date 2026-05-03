using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TechConf.Api.Hubs;

public interface IProposalNotificationsClient
{
    Task ProposalReviewed(ProposalReviewNotification notification);
}

public sealed record ProposalReviewNotification(
    int ProposalId,
    string Title,
    string Status,
    string Message,
    DateTimeOffset ReviewedAtUtc);

[Authorize]
public sealed class ProposalNotificationsHub : Hub<IProposalNotificationsClient>
{
    public override async Task OnConnectedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Context.UserIdentifier))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroup(Context.UserIdentifier));
        }

        await base.OnConnectedAsync();
    }

    public static string GetUserGroup(string userId) => $"user:{userId}";
}
