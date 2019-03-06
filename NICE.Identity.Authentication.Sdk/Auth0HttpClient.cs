using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace NICE.Identity.Authentication.Sdk
{
	internal class Auth0HttpClient : IAuth0HttpClient
	{
		private readonly HttpClient _client;
		private readonly ITokenService _tokenService;


		public Auth0HttpClient(HttpClient httpClient)
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
}
}