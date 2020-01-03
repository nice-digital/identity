


using Claim = System.Security.Claims.Claim;
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
using System.Linq;
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

				ResponseType = OpenIdConnectResponseType.CodeIdTokenToken, //Denotes the kind of credential that Auth0 will return (code vs token). For this flow (hybrid), the value must be code id_token, code token, or code id_token token. More specifically, token returns an Access Token, id_token returns an ID Token, and code returns the Authorization Code.
				Scope = "openid profile email offline_access",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = ClaimType.DisplayName,
					RoleClaimType = ClaimType.Role
				},
				CallbackPath = new PathString(authConfiguration.WebSettings.CallBackPath ?? "/signin-auth0"), //if this isn't passed, then it's just worked out from the RedirectUri
				SaveTokens = true,
				
				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					AuthorizationCodeReceived = notification =>
					{
						
						return Task.CompletedTask;
					},
					SecurityTokenValidated = async notification =>
					{
						var accessToken = notification.ProtocolMessage.AccessToken; //TODO: not sure this is right
						var userId = notification.AuthenticationTicket.Identity.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value; 
						var host = notification.Request.Host.Value;
						var httpClientToUse = httpClient ?? new HttpClient();
						var claimsToAdd = await ClaimsHelper.AddClaimsToUser(authConfiguration, userId, accessToken, new List<string> { host }, httpClientToUse);

						claimsToAdd.Add(new Claim("id_token", notification.ProtocolMessage.IdToken));
						claimsToAdd.Add(new Claim("access_token", notification.ProtocolMessage.AccessToken));

						notification.AuthenticationTicket.Identity.AddClaims(claimsToAdd);
					},

                    RedirectToIdentityProvider = notification =>
					{
						if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
						{
							if (!string.IsNullOrEmpty(authConfiguration.MachineToMachineSettings.ApiIdentifier))
							{
								notification.ProtocolMessage.SetParameter("audience", authConfiguration.MachineToMachineSettings.ApiIdentifier);
							}
							var dictionary = notification.OwinContext.Authentication.AuthenticationResponseChallenge?.Properties.Dictionary;
							if (dictionary != null && dictionary.ContainsKey("register"))
							{
								notification.ProtocolMessage.SetParameter("register", dictionary["register"]);
							}
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