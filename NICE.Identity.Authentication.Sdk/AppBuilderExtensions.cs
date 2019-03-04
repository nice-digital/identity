using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Owin.Security.DataProtection;
using NICE.Identity.Authentication.Sdk.Configurations;
using Rhasta.Owin.Security.Cookies.Store.Redis;
using TicketDataFormat = Microsoft.Owin.Security.DataHandler.TicketDataFormat;

namespace NICE.Identity.Authentication.Sdk
{
	public static class AppBuilderExtensions
	{
		public static void AddAuthentication(this IAppBuilder app, AuthConfiguration authConfiguration, RedisConfiguration redisConfiguration)
		{
			// Enable Kentor Cookie Saver middleware
			app.UseKentorOwinCookieSaver();
			IDataProtector dataProtector = app.CreateDataProtector(typeof(RedisAuthenticationTicket).FullName);
            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType);
			app.UseCookieAuthentication(options: new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
			{
				AuthenticationType = Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType,
				SessionStore = new RedisSessionStore(new TicketDataFormat(dataProtector), redisConfiguration),
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
