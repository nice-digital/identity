using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authentication
{
	public class AuthenticationService : IAuthenticationService
    {
        private const string AuthenticationScheme = "Auth0";

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
    }
}