using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.External
{
    internal sealed class HttpClientDecorator : IHttpClientDecorator
    {
        private readonly HttpClient _httpClient;

        public HttpClientDecorator(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

		/// <summary>
		/// this method throws a HttpRequestException if there's any problem like 404, 500.
		/// that's fine, except the status code is only then in the message so you'd have to parse a string to get the status code.
		/// </summary>
		/// <param name="requestUri"></param>
		/// <returns></returns>
        public async Task<string> GetStringAsync(Uri requestUri)
        {
	        return await _httpClient.GetStringAsync(requestUri);
        }

		/// <summary>
		/// this method returns a httpresponsemessage, which contains a status code.
		/// </summary>
		/// <param name="requestUri"></param>
		/// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
	        return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            return await _httpClient.PutAsync(requestUri, content);
        }

        public async Task<JwtToken> GetBearerToken(Uri requestUri, StringContent content)
        {
            var httpResponseMessageResponse = await _httpClient.PostAsync(requestUri, content);
            if (httpResponseMessageResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("An Error Occured");
            }
            var token = await httpResponseMessageResponse.Content.ReadAsAsync<JwtToken>();
            return token;
        }
        public void AddBearerToken(JwtToken token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
    }
}