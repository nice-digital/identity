using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.External
{
    internal interface IHttpClientDecorator
    {
        Task<string> GetStringAsync(string requestUri);
    }
}