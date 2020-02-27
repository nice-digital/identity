using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;

#if NETFRAMEWORK

using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Host.SystemWeb;

#else

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

#endif

namespace NICE.Identity.Authentication.Sdk.Authentication
{
	public class AuthenticationService : IAuthenticationService
    {

        public async Task Login(HttpContext context, string returnUrl = "/", bool goToRegisterPage = false)
        {
#if NETFRAMEWORK
	        var owinContext = new HttpContextWrapper(context).GetOwinContext();
	        owinContext.Authentication.Challenge(new AuthenticationProperties
		        {
			        RedirectUri = returnUrl ?? "/",
			        IsPersistent = true,
			        Dictionary = { { "register", goToRegisterPage.ToString().ToLower() } }
		        },
		        AuthenticationConstants.AuthenticationScheme);
	        //return new HttpUnauthorizedResult(); //todo: not sure about this.
#else
            await context.ChallengeAsync(AuthenticationConstants.AuthenticationScheme,
		        new AuthenticationProperties
		        {
			        RedirectUri = returnUrl,
			        Items = {new KeyValuePair<string, string>(nameof(goToRegisterPage), goToRegisterPage.ToString().ToLower())}
		        });
#endif
        }

        public async Task Logout(HttpContext context, string returnUrl = "/")
        {
#if NETFRAMEWORK
	        var owinContext = new HttpContextWrapper(context).GetOwinContext();
	        owinContext.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
	        owinContext.Authentication.SignOut(AuthenticationConstants.AuthenticationScheme);
#else
			const bool forceHttps = true; 
	        await context.SignOutAsync(AuthenticationConstants.AuthenticationScheme, new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = (new Uri(context.Request.GetUri(forceHttps), returnUrl)).AbsoluteUri
            });

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
#endif
		}
    }
}