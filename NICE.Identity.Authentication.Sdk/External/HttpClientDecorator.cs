using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.External
{
    internal sealed class HttpClientDecorator : IHttpClientDecorator
    {
        private readonly HttpClient _httpClient;

        public HttpClientDecorator(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            return await _httpClient.GetStringAsync(requestUri);
        }
    }
}