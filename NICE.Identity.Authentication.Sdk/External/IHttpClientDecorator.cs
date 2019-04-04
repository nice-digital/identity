using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.External
{
    public interface IHttpClientDecorator
    {
        Uri BaseUrl { get; }
        Task<string> GetStringAsync(Uri requestUri);

        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
    }
}