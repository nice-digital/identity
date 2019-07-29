using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.External;

namespace NICE.Identity.Authentication.Sdk.Authentication
{
	public class Auth0Service : Abstractions.IAuthenticationService
	{
		private readonly IAuthConfiguration _authConfiguration;
		private const string AuthenticationScheme = "Auth0";
		private readonly HttpClient _client;

		public Auth0Service(IHttpClientFactory client, IAuthConfiguration authConfiguration)
		{
			_authConfiguration = authConfiguration;
			_client = client.CreateClient("Auth0ServiceApiClient");
		}
		public async Task Login(HttpContext context, string returnUrl = "/")
		{
			await context.ChallengeAsync(AuthenticationScheme, new AuthenticationProperties { RedirectUri = returnUrl });
		}
        
		public async Task Logout(HttpContext context, string returnUrl = "/")
		{
			await context.SignOutAsync(AuthenticationScheme, new AuthenticationProperties
			{
				// Indicate here where Auth0 should redirect the user after a logout.
				// Note that the resulting absolute Uri must be whitelisted in the 
				// **Allowed Logout URLs** settings for the client.
				RedirectUri = returnUrl
			});

			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

		//public async Task<JwtToken> GetToken()
		//{
		//	var request = new
		//	{
		//		grant_type = _authConfiguration.GrantTypeForMachineToMachine,
		//		client_id = _authConfiguration.WebSettings.ClientId,
		//		client_secret = _authConfiguration.WebSettings.ClientSecret,
		//		audience = _authConfiguration.MachineToMachineSettings.ApiIdentifier,
		//		//redirect_uri = _authConfiguration.WebSettings.PostLogoutRedirectUri,
		//	};
		//	_client.BaseAddress = new Uri("todo");

		//	var httpResponseMessageResponse = await _client.PostAsync("oauth/token", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
		//	if (httpResponseMessageResponse.StatusCode != HttpStatusCode.OK)
		//	{
		//		throw new HttpRequestException("An Error Occured");
		//	}

		//	var token = await httpResponseMessageResponse.Content.ReadAsAsync<JwtToken>();

		//	return token;
		//}
	}
}