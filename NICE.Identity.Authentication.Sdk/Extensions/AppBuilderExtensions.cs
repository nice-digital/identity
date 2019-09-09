using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OpenIdConnect;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Redis;
using Owin;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AppBuilderExtensions
	{
		public static void AddOwinAuthentication(this IAppBuilder app, IAuthConfiguration authConfiguration, RedisConfiguration redisConfiguration)
		{
            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();
			var dataProtector = app.CreateDataProtector(typeof(RedisAuthenticationTicket).FullName);
            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            var options = new CookieAuthenticationOptions
            {
	            AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
	            SessionStore = new NiceRedisSessionStore(new TicketDataFormat(dataProtector), redisConfiguration),
	            CookieHttpOnly = true,
	            CookieSecure = CookieSecureOption.Always,
	            LoginPath = new PathString("/Account/Login")
            };

            app.UseCookieAuthentication(options);

            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				AuthenticationType = "Auth0",

				Authority = $"https://{authConfiguration.TenantDomain}",

				ClientId = authConfiguration.WebSettings.ClientId,
				ClientSecret = authConfiguration.WebSettings.ClientSecret,

				RedirectUri = authConfiguration.WebSettings.RedirectUri,
				PostLogoutRedirectUri = authConfiguration.WebSettings.PostLogoutRedirectUri,

				ResponseType = OpenIdConnectResponseType.CodeIdTokenToken,
				Scope = "openid profile",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name"
				},

				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					SecurityTokenValidated = notification =>
					{
						notification.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", notification.ProtocolMessage.IdToken));
						notification.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", notification.ProtocolMessage.AccessToken));

						return Task.FromResult(0);
					},

                    RedirectToIdentityProvider = notification =>
					{
						if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
						{
							notification.ProtocolMessage.SetParameter("audience", authConfiguration.MachineToMachineSettings.ApiIdentifier);
						}
						else if(notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
						{
							var logoutUri = $"https://{authConfiguration.TenantDomain}/v2/logout?client_id={authConfiguration.WebSettings.ClientId}";

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
