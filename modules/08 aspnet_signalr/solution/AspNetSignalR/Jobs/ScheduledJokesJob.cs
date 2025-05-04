using AspNetSignalR.Hubs;
using AspNetSignalR.Models;
using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;

namespace AspNetSignalR.Jobs;

public class ScheduledJokesJob(HttpClient httpClient, IHubContext<ChatHub, IChatHub> hub) : IInvocable
{
    public async Task Invoke()
    {
        var joke = await httpClient.GetFromJsonAsync<Joke>("https://api.chucknorris.io/jokes/random");
        await hub.Clients.All.ReceiveJoke(joke);
    }
}