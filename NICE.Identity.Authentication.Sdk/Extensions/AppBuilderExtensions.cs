#if NETFRAMEWORK //This whole class is only used by .net framework. 
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using NICE.Identity.Authentication.Sdk.SessionStore;
using NICE.Identity.Authentication.Sdk.Tracking;
using Claim = System.Security.Claims.Claim;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AppBuilderExtensions
	{
		public static void AddOwinAuthentication(this IAppBuilder app, IAuthConfiguration authConfiguration, HttpClient httpClient = null) //, RedisConfiguration redisConfiguration)
		{
			var localHttpClient = httpClient ?? new HttpClient();

			// Enable Kentor Cookie Saver middleware https://coding.abel.nu/2014/11/catching-the-system-webowin-cookie-monster/
			app.UseKentorOwinCookieSaver();

			// Set Cookies as default authentication type
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			var options = new CookieAuthenticationOptions
			{
				AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
				SessionStore = authConfiguration.RedisConfiguration != null && authConfiguration.RedisConfiguration.Enabled ? new RedisOwinSessionStore(new TicketDataFormat(app.CreateDataProtector(typeof(RedisAuthenticationTicket).FullName)), authConfiguration.RedisConfiguration.ConnectionString) : null,
				CookieHttpOnly = true,
				CookieSecure = CookieSecureOption.Always,
                CookieSameSite = SameSiteMode.None,
				LoginPath = new PathString("/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					OnValidateIdentity = async context =>
					{
						if (context.Identity.IsAuthenticated)
						{
							//the access token, refresh token and access token expiration are stored in properties initially i.e. in the cookie. after a refresh though, they're stored as claims in the cookie. as i can't see how to get the cookie to update like it does in .net core.

							var refreshToken = context.Identity.Claims.FirstOrDefault(claim => claim.Type.Equals(AuthenticationConstants.Tokens.RefreshToken))?.Value ??
								(context.Properties.Dictionary.ContainsKey(AuthenticationConstants.Tokens.RefreshToken) ? context.Properties.Dictionary[AuthenticationConstants.Tokens.RefreshToken] : null);
							
							var accessTokenExpires = context.Identity.Claims.FirstOrDefault(claim => claim.Type.Equals(AuthenticationConstants.Tokens.AccessTokenExpires))?.Value ??
								(context.Properties.Dictionary.ContainsKey(AuthenticationConstants.Tokens.AccessTokenExpires) ? context.Properties.Dictionary[AuthenticationConstants.Tokens.AccessTokenExpires] : null);

							if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessTokenExpires)) //this should never really happen. it's just a safety check.
							{
								context.RejectIdentity(); //reject will issue 401. if the client app allows anonymous, then they will simply be logged out. if the route needs authentication then they will be redirected back to the login page
								return;
							}

							var expiryDateUtc = DateTime.Parse(accessTokenExpires).ToUniversalTime();
							if (expiryDateUtc < DateTime.UtcNow)
							{
								//update access token here using refresh token (if it's not been revoked)
								var refreshTokenResponse = await ClaimsHelper.UpdateAccessToken(authConfiguration, refreshToken, localHttpClient);
								if (!refreshTokenResponse.Valid)
								{
									context.RejectIdentity(); //refresh token has been revoked. this will happen when the user's roles are changed in the admin site.
									return;
								}

								context.Properties.AllowRefresh = true;
								var newExpiryDate = DateTime.UtcNow.AddSeconds(refreshTokenResponse.ExpiresInSeconds);

								var identity = context.Identity;

								identity.TryRemoveClaim(identity.Claims.FirstOrDefault(claim => claim.Type.Equals(AuthenticationConstants.Tokens.AccessToken)));
								identity.AddClaim(new Claim(AuthenticationConstants.Tokens.AccessToken, refreshTokenResponse.AccessToken));

								identity.TryRemoveClaim(identity.Claims.FirstOrDefault(claim => claim.Type.Equals(AuthenticationConstants.Tokens.AccessTokenExpires)));
								identity.AddClaim(new Claim(AuthenticationConstants.Tokens.AccessTokenExpires, newExpiryDate.ToString("o", CultureInfo.InvariantCulture)));

								identity.TryRemoveClaim(identity.Claims.FirstOrDefault(claim => claim.Type.Equals(AuthenticationConstants.Tokens.RefreshToken)));
								identity.AddClaim(new Claim(AuthenticationConstants.Tokens.RefreshToken, refreshToken));

								context.ReplaceIdentity(identity); 
								//context.Request.Context.Authentication.SignIn(identity); //alternate method. doesn't appear to be necessary.
							}
						}
					}
				}
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

				ResponseType = OpenIdConnectResponseType.Code, //Denotes the kind of credential that Auth0 will return (code vs token). For this flow (code), the value must be code id_token, code token, or code id_token token. More specifically, token returns an Access Token, id_token returns an ID Token, and code returns the Authorization Code.
				ResponseMode = OpenIdConnectResponseMode.Query, //needed for authorisation code flow.
				RedeemCode = true, //needed for authorisation code flow.
				Scope = "openid profile email offline_access",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = ClaimType.DisplayName,
					RoleClaimType = ClaimType.Role
				},
				CallbackPath = new PathString(authConfiguration.WebSettings.CallBackPath ?? "/signin-auth0"), //if this isn't passed, then it's just worked out from the RedirectUri
				SaveTokens = true, //stores tokens in the cookies. but only accessible by the server, not the client

				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					SecurityTokenValidated = async notification =>
					{
						var accessToken = notification.ProtocolMessage.AccessToken;
						var userId = notification.AuthenticationTicket.Identity.Claims
							.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
						var host = notification.Request.Host.Value;
						var claimsToAdd = await ClaimsHelper.AddClaimsToUser(authConfiguration, userId, accessToken,
							new List<string> {host}, localHttpClient);

						claimsToAdd.Add(new Claim("expires_in", notification.ProtocolMessage.ExpiresIn));

						claimsToAdd.Add(new Claim("id_token", notification.ProtocolMessage.IdToken)); //TODO: not sure if we should add these. 
						claimsToAdd.Add(new Claim(AuthenticationConstants.Tokens.AccessToken, notification.ProtocolMessage.AccessToken));
						
						notification.AuthenticationTicket.Identity.AddClaims(claimsToAdd);

						var cookies = notification.Request.Cookies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
						TrackingService.TrackSuccessfulSignIn(localHttpClient, cookies, notification.Request.Host.Value, authConfiguration.GoogleTrackingId);
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
						else if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
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
								logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
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