using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspNetCoreSecurity.ConsoleClient
{
    class Program
    {
        private static HttpClient _client = new HttpClient();
        
        static async Task Main(string[] args)
        {
            var tokenResponse = await GetAuthToken(_client);

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            await MakeUnauthorizedRequest(_client);

            await MakeAuthorizedRequest(_client, tokenResponse);
        }

        private static async Task<TokenResponse> GetAuthToken(HttpClient client)
        {
            var tokenEndpoint = "https://localhost:44386/connect/token";
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = "trusted-client",
                ClientSecret = "secret",

                UserName = "Katie",
                Password = "Pass123$",
                Scope = "api1"
            });
            return tokenResponse;
        }

        private static async Task MakeAuthorizedRequest(HttpClient client, TokenResponse tokenResponse)
        {
            client.SetBearerToken(tokenResponse.AccessToken);
            var weatherResponse = await client.GetAsync("https://localhost:44387/api/sampledata/weatherforecasts");

            var text = await client.GetStringAsync("https://localhost:44387/api/sampledata/weatherforecasts");

            var forecasts = await weatherResponse.Content.ReadAsAsync<Forecast[]>();

            foreach (var forecast in forecasts)
            {
                Console.WriteLine(forecast.Summary);
            }
        }

        private static async Task MakeUnauthorizedRequest(HttpClient client)
        {
            var weatherResponse = await client.GetAsync("https://localhost:44387/api/sampledata/weatherforecasts");
            Console.WriteLine(weatherResponse.StatusCode);
        }
    }

    internal class Forecast
    {
        public string DateFormatted { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}