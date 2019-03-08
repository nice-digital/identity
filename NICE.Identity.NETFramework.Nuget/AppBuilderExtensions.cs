using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace NICE.Identity.NETFramework.Nuget
{
	public static class AppBuilderExtensions
	{
		

		public static void AddAuthentication(this IAppBuilder app, AuthConfiguration authConfiguration)
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

				Authority = $"https://{authConfiguration.Domain}",

				ClientId = authConfiguration.ClientId,
				ClientSecret = authConfiguration.ClientSecret,

				RedirectUri = authConfiguration.RedirectUri,
				PostLogoutRedirectUri = authConfiguration.PostLogoutRedirectUri,

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
							var logoutUri = $"https://{authConfiguration.Domain}/v2/logout?client_id={authConfiguration.ClientId}";

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
