#if NETSTANDARD //This whole class is only used by .net framework. we target .net standard 2.0 which is compatible with .net framework 4.6.1
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OpenIdConnect;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using Owin;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AppBuilderExtensions
	{
		public static void AddOwinAuthentication(this IAppBuilder app, IAuthConfiguration authConfiguration, HttpClient httpClient = null) //, RedisConfiguration redisConfiguration)
		{
            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();

			
			//var dataProtector = app.CreateDataProtector(typeof(RedisAuthenticationTicket).FullName);
            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            var options = new CookieAuthenticationOptions
            {
	            AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
	          //  SessionStore = new RedisOwinSessionStore(new TicketDataFormat(dataProtector), redisConfiguration),
	            CookieHttpOnly = true,
	            CookieSecure = CookieSecureOption.Always,
	            LoginPath = new PathString("/Account/Login")
            };

            app.UseCookieAuthentication(options);

            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				AuthenticationType = AuthenticationConstants.AuthenticationScheme,

				Authority = $"https://{authConfiguration.TenantDomain}",

				ClientId = authConfiguration.WebSettings.ClientId,
				ClientSecret = authConfiguration.WebSettings.ClientSecret,

				RedirectUri = authConfiguration.WebSettings.RedirectUri,
				PostLogoutRedirectUri = authConfiguration.WebSettings.PostLogoutRedirectUri,

				ResponseType = OpenIdConnectResponseType.CodeIdToken, //Denotes the kind of credential that Auth0 will return (code vs token). For this flow (hybrid), the value must be code id_token, code token, or code id_token token. More specifically, token returns an Access Token, id_token returns an ID Token, and code returns the Authorization Code.
				Scope = "openid profile email offline_access",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name"
				},
				CallbackPath = new PathString(authConfiguration.WebSettings.CallBackPath ?? "/signin-auth0"), //if this isn't passed, then it's just worked out from the RedirectUri
				SaveTokens = true,
				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					SecurityTokenValidated = async notification =>
					{
						//notification.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", notification.ProtocolMessage.IdToken));
						//notification.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", notification.ProtocolMessage.AccessToken));

						//TODO: validate this!

						var accessToken = notification.ProtocolMessage.AccessToken;
						var userId = notification.ProtocolMessage.UserId; 
						var host = notification.Request.Host.Value;
						var httpClientToUse = httpClient ?? new HttpClient();
						ClaimsIdentity user = notification.AuthenticationTicket.Identity;

						//todo: need a ClaimsPrincipal, not ClaimsIdentity..
						//await ClaimsHelper.AddClaimsToUser(authConfiguration, userId, accessToken, new List<string> { host }, notification.AuthenticationTicket.Identity, httpClientToUse);
						
					},

                    RedirectToIdentityProvider = notification =>
					{
						if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
						{
							if (!string.IsNullOrEmpty(authConfiguration.MachineToMachineSettings.ApiIdentifier))
							{
								notification.ProtocolMessage.SetParameter("audience", authConfiguration.MachineToMachineSettings.ApiIdentifier);
							}

							//TODO: implement this:
							//if (notification.Properties.Items.ContainsKey("goToRegisterPage"))
							//{
							//	context.ProtocolMessage.SetParameter("register", context.Properties.Items["goToRegisterPage"]);
							//}
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

#endif