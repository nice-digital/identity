using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Abstractions;

namespace NICE.Identity.Authentication.Sdk.External
{
	internal sealed class HttpClientDecorator : IHttpClientDecorator
	{
		private readonly HttpClient _httpClient;

		public HttpClientDecorator(HttpClient httpClient)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public async Task<string> GetStringAsync(Uri requestUri)
		{
			return await _httpClient.GetStringAsync(requestUri);
		}

		public async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
		{
			return await _httpClient.PutAsync(requestUri, content);
		}

		public void AddBearerToken(JwtToken token)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
		}
	}
}