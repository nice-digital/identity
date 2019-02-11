using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Abstractions;

namespace NICE.Identity.Authentication.Sdk
{
	public class NICEAuthenticationService : INICEAuthenticationService
	{
		public async Task Login(HttpContext context, string returnUrl = "/")
		{
			await context.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
		}

		[Authorize]
		public async Task Logout(HttpContext context, string returnUrl = "/")
		{
			await context.SignOutAsync("Auth0", new AuthenticationProperties
			{
				// Indicate here where Auth0 should redirect the user after a logout.
				// Note that the resulting absolute Uri must be whitelisted in the 
				// **Allowed Logout URLs** settings for the client.
				RedirectUri = returnUrl
			});
			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
