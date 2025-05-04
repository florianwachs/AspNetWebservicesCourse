using AspNetSignalR.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspNetSignalR.Hubs;

public class ChatHub : Hub<IChatHub>
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }
}

public interface IChatHub
{
    Task ReceiveMessage(string user, string message);
    Task ReceiveJoke(Joke joke);
}