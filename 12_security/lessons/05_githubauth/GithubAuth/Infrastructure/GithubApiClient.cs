using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GithubAuth.Infrastructure
{
    public class GithubApiClient
    {
        public GithubApiClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        public HttpClient Client { get; }

        public async Task<UserInfo> GetUserInfo(string token)
        {
            var userInfoResponse = await Client.SendAsync(GetWithToken(token, "user"));
            userInfoResponse.EnsureSuccessStatusCode();
            var result = await userInfoResponse.Content.ReadFromJsonAsync<UserInfo>();

            return result;
        }

        private static HttpRequestMessage GetWithToken(string token, string urlPart)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, urlPart);
            request.Headers.Authorization = new AuthenticationHeaderValue("token", token);
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("GithubTest", "v1"));
            return request;
        }

        public record UserInfo(string Login, long Id, string Avatar_url, string Name, string Company);
    }
}
