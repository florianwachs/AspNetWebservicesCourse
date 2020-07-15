using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAuth.Infrastructure
{
    public class DiscordApiClient
    {
        private HttpClient Client { get; }
        public DiscordApiClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://discord.com/api/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        public async Task<UserInfo> GetUserInfo(string token)
        {
            var guildsResponse = await Client.SendAsync(GetWithToken(token, "users/@me/guilds"));
            guildsResponse.EnsureSuccessStatusCode();
            var guilds = await guildsResponse.Content.ReadFromJsonAsync<Guild[]>();

            var userInfoResponse = await Client.SendAsync(GetWithToken(token, "users/@me"));
            userInfoResponse.EnsureSuccessStatusCode();
            var result = await userInfoResponse.Content.ReadFromJsonAsync<UserInfo>();

            result.Guilds = guilds;
            return result;
        }

        private static HttpRequestMessage GetWithToken(string token, string urlPart)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, urlPart);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }


        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy.HandleResult<HttpResponseMessage>(a => a.StatusCode == (HttpStatusCode)429)
                .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryCount, response, context) =>
                {
                    // Normal könnten wir RetryAfter.Delta verwenden, Discord nimmt aber nicht Sekunden sondern Millisekunden
                    // Daher müssen wir den Wert manuell parsen
                    return GetRetryForDiscord(response.Result);
                },
           onRetryAsync: async (response, timespan, retryCount, context) =>
           {
               /* perhaps some logging, eg the retry count, the timespan delaying for */
           });
        }

        private static TimeSpan GetRetryForDiscord(HttpResponseMessage response)
        {
            int maxWaitTimeInMs = 1000;
            int waitTimeInMs;
            if (!int.TryParse(response?.Headers.RetryAfter?.ToString(), out waitTimeInMs))
            {
                waitTimeInMs = maxWaitTimeInMs;
            }

            return TimeSpan.FromMilliseconds(Math.Min(waitTimeInMs, maxWaitTimeInMs));
        }
    }


    public class UserInfo
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
        public IReadOnlyCollection<Guild> Guilds { get; set; }
    }

    public partial class Guild
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool Owner { get; set; }
        public long Permissions { get; set; }
    }
}
