using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk
{
	public static class AppBuilderExtensions
	{
		public static void AddAuthentication(this IAppBuilder app, NameValueCollection appSettings)
		{
			AddAuthentication(app, appSettings["Domain"], appSettings["ClientId"], appSettings["ClientSecret"], appSettings["RedirectUri"], appSettings["PostLogoutRedirectUri"]);
		}

		public static void AddAuthentication(this IAppBuilder app, string domain, string clientId, string clientSecret, string redirectURI, string postLogoutRedirectURI)
		{

			// Enable Kentor Cookie Saver middleware
			app.UseKentorOwinCookieSaver();

			// Set Cookies as default authentication type
			app.SetDefaultSignInAsAuthenticationType(Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType);
			app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
			{
				AuthenticationType = Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType,
				LoginPath = new Microsoft.Owin.PathString("/Account/Login")
			});

			// Configure Auth0 authentication
			app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				AuthenticationType = "Auth0",

				Authority = $"https://{domain}",

				ClientId = clientId,
				ClientSecret = clientSecret,

				RedirectUri = redirectURI,
				PostLogoutRedirectUri = postLogoutRedirectURI,

				ResponseType = OpenIdConnectResponseType.CodeIdToken,
				Scope = "openid profile",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name"
				},

				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					RedirectToIdentityProvider = notification =>
					{
						if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
						{
							var logoutUri = $"https://{domain}/v2/logout?client_id={clientId}";

							var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
							if (!string.IsNullOrEmpty(postLogoutUri))
							{
								if (postLogoutUri.StartsWith("/"))
								{
									// transform to absolute
									var request = notification.Request;
									postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
								}
								logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
							}

							notification.Response.Redirect(logoutUri);
							notification.HandleResponse();
						}
						return Task.FromResult(0);
					}
				}
			});

		}
	}
}
