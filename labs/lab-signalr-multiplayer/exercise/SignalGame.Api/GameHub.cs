using Microsoft.AspNetCore.SignalR;

namespace SignalGame.Api;

public sealed class GameHub(GameState state) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("StateChanged", state.Snapshot());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var snapshot = state.Disconnect(Context.ConnectionId);
        if (snapshot is not null)
        {
            await Clients.All.SendAsync("StateChanged", snapshot);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string playerName)
    {
        var normalizedName = NormalizePlayerName(playerName);

        QuizSnapshot snapshot;
        try
        {
            snapshot = state.Join(normalizedName, Context.ConnectionId);
        }
        catch (InvalidOperationException ex)
        {
            throw new HubException(ex.Message);
        }

        await Clients.All.SendAsync("StateChanged", snapshot);
        await Clients.Others.SendAsync("PlayerJoined", normalizedName);
    }

    public Task StartGame() => BroadcastState(state.StartGame);

    public Task SubmitAnswer(int optionIndex) => BroadcastState(() => state.SubmitAnswer(Context.ConnectionId, optionIndex));

    public Task NextQuestion() => BroadcastState(state.NextQuestion);

    public Task ResetGame() => BroadcastState(state.ResetGame);

    private async Task BroadcastState(Func<QuizSnapshot> action)
    {
        try
        {
            var snapshot = action();
            await Clients.All.SendAsync("StateChanged", snapshot);
        }
        catch (InvalidOperationException ex)
        {
            throw new HubException(ex.Message);
        }
    }

    private static string NormalizePlayerName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            throw new HubException("A player name is required.");
        }

        var normalized = playerName.Trim();
        if (normalized.Length is < 2 or > 20)
        {
            throw new HubException("Player names must be between 2 and 20 characters.");
        }

        return normalized;
    }
}
