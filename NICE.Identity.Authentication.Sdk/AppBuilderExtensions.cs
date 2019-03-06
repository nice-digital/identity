using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OpenIdConnect;
using NICE.Identity.Authentication.Sdk.Configurations;
using Owin;
using Rhasta.Owin.Security.Cookies.Store.Redis;

namespace NICE.Identity.Authentication.Sdk
{
	public static class AppBuilderExtensions
	{
		public static void AddAuthentication(this IAppBuilder app, string clientName, AuthConfiguration authConfiguration, RedisConfiguration redisConfiguration)
		{
            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();
			var dataProtector = app.CreateDataProtector(typeof(RedisAuthenticationTicket).FullName);
            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            var options = new CookieAuthenticationOptions
            {
	            AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
				AuthenticationMode = AuthenticationMode.Active,
	            SessionStore = new RedisSessionStore(new TicketDataFormat(dataProtector), redisConfiguration),
	            CookieHttpOnly = true,
	            CookieSecure = CookieSecureOption.Always,
                CookieName = $"{clientName}.Data-Protection",
	            ExpireTimeSpan = TimeSpan.FromMinutes(30),
	            LoginPath = new PathString("/Account/Login")
            };

            app.UseCookieAuthentication(options, PipelineStage.PreHandlerExecute);

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
