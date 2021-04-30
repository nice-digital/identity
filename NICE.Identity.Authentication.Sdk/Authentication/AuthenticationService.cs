#if NETSTANDARD2_0 || NETCOREAPP3_1
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.Authentication.Sdk.API;
using System.Security.Claims;

namespace NICE.Identity.Authentication.Sdk.Authentication
{
	public class AuthenticationService : IAuthenticationService
    {
        private readonly IAPIService _apiService;

        public AuthenticationService(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task Login(HttpContext context, string returnUrl = "/", bool goToRegisterPage = false)
        {
	        await context.ChallengeAsync(AuthenticationConstants.AuthenticationScheme,
		        new AuthenticationProperties
		        {
			        RedirectUri = returnUrl,
			        Items = {new KeyValuePair<string, string>(nameof(goToRegisterPage), goToRegisterPage.ToString().ToLower())}
		        });
        }

        public async Task Logout(HttpContext context, string returnUrl = "/")
        {
	        const bool forceHttps = true; 
	        await context.SignOutAsync(AuthenticationConstants.AuthenticationScheme, new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = (new Uri(context.Request.GetUri(forceHttps), returnUrl)).AbsoluteUri
            });

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //temporarily removing to see if it resolves the indev p2 logout issue. If it does we could implement this feature in the new profile page/site, by having a button which signs you out of all other sessions.
            //var userId = context.User.Claims.Single(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            //await _apiService.RevokeRefreshTokensForUser(userId);
        }
    }
}
#endif