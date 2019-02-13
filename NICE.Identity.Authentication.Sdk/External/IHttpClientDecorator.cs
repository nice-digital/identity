using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.External
{
    internal interface IHttpClientDecorator
    {
        Task<string> GetStringAsync(Uri requestUri);

        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
    }
}