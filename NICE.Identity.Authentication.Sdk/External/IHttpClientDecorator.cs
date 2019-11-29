using System;
using System.Net.Http;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.External
{
	public interface IHttpClientDecorator
	{
		Task<string> GetStringAsync(Uri requestUri);
		Task<HttpResponseMessage> GetAsync(Uri requestUri);
		Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
  //      Task<JwtToken> GetBearerToken(Uri requestUri, StringContent content);
		//void AddBearerToken(string accessToken);
    }
}