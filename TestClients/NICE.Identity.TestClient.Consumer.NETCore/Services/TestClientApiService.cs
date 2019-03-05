using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

	    public HttpResponseMessage Send(HttpRequestMessage request)
	    {
		    return _client.SendAsync(request).Result;
	    }

	    private string GetString(HttpResponseMessage response)
	    {
		    var content = response.Content;
		    var readTask = content.ReadAsStringAsync();
		    return readTask.Result;
	    }

		public Publication GetPublication(string url, JwtToken token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"{token.token_type} {token.access_token}");

            var response = Send(request);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpRequestException("An Error Occured");
			}

			var content = GetString(response);
            var publication = JsonConvert.DeserializeObject<Publication>(content);

            return publication;
        }
    }
}

    public class Publication
    {
        public string Id { get; set; }

        public string SomeText { get; set; }
    }
}