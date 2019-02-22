using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace NICE.Identity.TestClient.M2MApp.Services
{
    public interface ITokenService
    {
        Task<JwtToken> GetToken();
    }

    public class Auth0TokenService : ITokenService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public Auth0TokenService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<JwtToken> GetToken()
        {
            var clientId = _config["Auth0:ClientId"];
            var clientSecret = _config["Auth0:ClientSecret"];
            var apiId = _config["Auth0:ApiIdentifier"];
            var domain = _config["Auth0:Domain"];

            string url = $"https://{domain}/oauth/token";

            JwtToken token;

            try
            {

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                //request.Headers.Add("content-type", "application/json");
                request.Content = new StringContent("{\"grant_type\":\"client_credentials\"," +
                                                    "\"client_id\": \"" + clientId + "\"," +
                                                    "\"client_secret\": \"" + clientSecret + "\"," +
                                                    "\"audience\": \"" + apiId + "\"}",
                    Encoding.UTF8,
                    "application/json");

                var result = await _client.SendAsync(request);
                string resultString = await result.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<JwtToken>(resultString);
            }
            catch (Exception e)
            {
                throw e;
            }

            return token;
        }
    }

    public class JwtToken
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }
    }
}