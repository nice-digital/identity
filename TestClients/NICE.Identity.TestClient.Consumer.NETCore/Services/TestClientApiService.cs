using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Identity.TestClient.M2MApp.Common;
using NICE.Identity.TestClient.M2MApp.Models;

namespace NICE.Identity.TestClient.M2MApp.Services
{
    public interface ITestClientApiService
    {
        Publication GetPublication(string url, JwtToken token);
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

		public Publication GetPublication(string url, JwtToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"{token.TokenType} {token.AccessToken}");

            var response = HttpResponseHelpers.Send(_client, request);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpRequestException("An Error Occured");
			}

			var content = HttpResponseHelpers.GetString(response);
            var publication = JsonConvert.DeserializeObject<Publication>(content);

            return publication;
        }
    }


    public class Publication
    {
        public string Id { get; set; }

        public string SomeText { get; set; }
    }
}