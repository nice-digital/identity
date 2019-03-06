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

namespace NICE.Identity.Authentication.Sdk.Authentication
{
	internal class Auth0Service : Abstractions.IAuthenticationService
	{
	    private const string AuthenticationScheme = "Auth0";
		private readonly HttpClient _client;
		private readonly IAuth0Configuration _auth0Configuraton;


		public Auth0Service(IHttpClientFactory client, IAuth0Configuration auth0Configration)
		{
			_client = client.CreateClient("Auth0ServiceApiClient");
			_auth0Configuraton = auth0Configration;
		}
		public async Task Login(HttpContext context, string returnUrl = "/")
		{
			await context.ChallengeAsync(AuthenticationScheme, new AuthenticationProperties() { RedirectUri = returnUrl });
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

		public async Task<JwtToken> GetToken()
		{
			var request = new
			{
				grant_type = _auth0Configuraton.GrantType,
				client_id = _auth0Configuraton.ClientId,
				client_secret = _auth0Configuraton.ClientSecret,
				audience = _auth0Configuraton.ApiIdentifier
			};

			var httpResponseMessageresponse = await _client.PostAsync("oauth/token", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
			if (httpResponseMessageresponse.StatusCode != HttpStatusCode.OK)
			{
				throw new HttpRequestException("An Error Occured");
			}

			var token = await httpResponseMessageresponse.Content.ReadAsAsync<JwtToken>();

			return token;
		}
	}
}