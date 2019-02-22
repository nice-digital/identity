using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NICE.Identity.TestClient.M2MApp.Services
{
    public interface ITestClientApiService
    {
        Task<Publication> GetPublication();
    }

    public class TestClientApiService : ITestClientApiService
    {
        private readonly HttpClient _client;
        private readonly ITokenService _tokenService;

        public TestClientApiService(HttpClient client, ITokenService tokenService)
        {
            _client = client;
            _tokenService = tokenService;
        }
        
        public async Task<Publication> GetPublication()
        {
            JwtToken token;
            try
            {
                token = await _tokenService.GetToken();
            }
            catch (Exception e)
            {
                throw e;
            }

            string url = "https://localhost:5001/api/publication";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("authorization", $"{token.token_type} {token.access_token}");
            request.Headers.Add("content-type", "application/json");

            var result = await _client.SendAsync(request);
            string resultString = await result.Content.ReadAsStringAsync();

            var publiction = JsonConvert.DeserializeObject<Publication>(resultString);

            return publiction;
        }
    }

    public class Publication
    {
        public string Id { get; set; }

        public string SomeText { get; set; }
    }
}