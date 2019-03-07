using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.NETFramework.Nuget.External
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
    }
}