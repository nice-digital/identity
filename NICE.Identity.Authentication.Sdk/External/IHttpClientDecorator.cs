using System;
using System.Net.Http;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Abstractions;

namespace NICE.Identity.Authentication.Sdk.External
{
	public interface IHttpClientDecorator
	{
		Task<string> GetStringAsync(Uri requestUri);

		Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);

		void AddBearerToken(JwtToken token);
	}
}