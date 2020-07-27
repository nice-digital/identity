#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.SessionStore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Tracking;
using AuthenticationService = NICE.Identity.Authentication.Sdk.Authentication.AuthenticationService;
using IAuthenticationService = NICE.Identity.Authentication.Sdk.Authentication.IAuthenticationService;
using static NICE.Identity.Authentication.Sdk.Constants;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IAuthConfiguration authConfiguration, HttpClient httpClient = null)
        {
            services.AddSingleton(authConfig => authConfiguration);
			services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.TryAddScoped<IAPIService, APIService>();
            services.AddHttpContextAccessor();
	        services.AddHttpClient(); //this adds http client factory for use in DI services like RoleRequirementHandler
			var localClient = httpClient ?? new HttpClient(); //this http client is used by this extension method only

			// Add authentication services
			services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
		        options.LoginPath = new PathString(authConfiguration.LoginPath);
		        options.LogoutPath = new PathString(authConfiguration.LogoutPath);

	            options.Cookie.Name = AuthenticationConstants.CookieName;
	            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.None;
				if (authConfiguration.RedisConfiguration != null && authConfiguration.RedisConfiguration.Enabled)
	            {
		            options.SessionStore = new RedisCacheTicketStore(new RedisCacheOptions { Configuration = authConfiguration.RedisConfiguration.ConnectionString });
                }
				options.Events = new CookieAuthenticationEvents
				{
					OnRedirectToAccessDenied = context =>
					{
						if (context.HttpContext.User.Identity.IsAuthenticated && !context.Response.HasStarted)
						{
							context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
						}
                        return Task.CompletedTask;
                    },
                    OnValidatePrincipal = async (context) =>
                    {
                        //check to see if user is authenticated first
                        if (context.Principal.Identity.IsAuthenticated)
                        {
                            //get the users tokens
                            var tokens = context.Properties.GetTokens().ToList();
                            var refreshToken = tokens.FirstOrDefault(t => t.Name.Equals(AuthenticationConstants.Tokens.RefreshToken));
                            var accessToken = tokens.FirstOrDefault(t => t.Name.Equals(AuthenticationConstants.Tokens.AccessToken));
                            var accessTokenExpires = tokens.FirstOrDefault(t => t.Name.Equals(AuthenticationConstants.Tokens.AccessTokenExpires));

                            if (string.IsNullOrEmpty(refreshToken?.Value) || string.IsNullOrEmpty(accessToken?.Value) || string.IsNullOrEmpty(accessTokenExpires?.Value)) //this should never really happen. it's just a safety check.
                            {
                                context.RejectPrincipal(); //reject will issue 401. if the client app allows anonymous, then they will simply be logged out. if the route needs authentication then they will be redirected back to the login page
                                return;
                            }
                            var expiryDateUtc = DateTime.Parse(accessTokenExpires.Value).ToUniversalTime(); //accessTokenExpires.Value contains a time like: "2019-12-08T13:00:43.8211482Z" the Z at the end indicates it's a UTC time. the DateTime.Parse converts it to local time. the ToUniversalTime converts back to UTC.
                            if (expiryDateUtc < DateTime.UtcNow)
                            {
                                //access token in the cookie has expired. There is a refresh token, so attempt to use the refresh token here and get another access token.
                                //this could still be rejected if the refresh token has been revoked at auth0.
                                var refreshTokenResponse = await ClaimsHelper.UpdateAccessToken(authConfiguration, refreshToken.Value, localClient);
                                if (!refreshTokenResponse.Valid)
                                {
                                    context.RejectPrincipal(); //this should only be hit if the user's access has been revoked.
                                    return;
                                }
                                accessToken.Value = refreshTokenResponse.AccessToken;
                                var newExpiryDate = DateTime.UtcNow.AddSeconds(refreshTokenResponse.ExpiresInSeconds);
                                accessTokenExpires.Value = newExpiryDate.ToString("o", CultureInfo.InvariantCulture);
                                context.Properties.StoreTokens(tokens);
                                context.ShouldRenew = true; //trigger context to renew cookie with new token values
                            }
                        }
                    }
                };

            })
            .AddOpenIdConnect(AuthenticationConstants.AuthenticationScheme, options =>
            {
	            // Set the authority to your Auth0 domain
                options.Authority = $"https://{authConfiguration.TenantDomain}";
                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = authConfiguration.WebSettings.ClientId;
                options.ClientSecret = authConfiguration.WebSettings.ClientSecret;
                // Set response type to code
                options.ResponseType = "code";
                // Configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                // Enables Refresh Tokens
                options.Scope.Add("offline_access");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimType.DisplayName,
                    RoleClaimType = ClaimType.Role
                };
                // Set the callback path, so Auth0 will call back to http://URI/signin-auth0
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                options.CallbackPath = new PathString(authConfiguration.WebSettings.CallBackPath ?? "/signin-auth0");
                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = "Auth0";
                // Saves tokens to the AuthenticationProperties
                options.SaveTokens = true;
                options.Events = new OpenIdConnectEvents
                {  
                    OnTokenValidated = async (context) =>
                    {
                        var accessToken = context.TokenEndpointResponse.AccessToken;
                        var userId = context.SecurityToken.Subject;
                        var claimsToAdd = await ClaimsHelper.AddClaimsToUser(authConfiguration, userId, accessToken, new List<string> { context.HttpContext.Request.Host.Host }, localClient);
                        context.Principal.AddIdentity(new ClaimsIdentity(claimsToAdd, null, ClaimType.DisplayName, ClaimType.Role));

                        var googleClientId = string.Empty;
                        if(context.SecurityToken.Payload.Any(t => t.Key == IdTokenPayload.tempCid))
                            googleClientId = context.SecurityToken.Payload.FirstOrDefault(t => t.Key == IdTokenPayload.tempCid).Value.ToString();
                        TrackingService.TrackSuccessfulSignIn(localClient, context.HttpContext.Request.Host.Value, authConfiguration.GoogleTrackingId, googleClientId);

                        // context.Success(); //don't do this, it causes a redirect loop
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        // Set audience if ApiIdentifier is present.
                        // This will add an access token that will enable api calls.
                        if (!string.IsNullOrEmpty(authConfiguration.MachineToMachineSettings.ApiIdentifier))
                        {
                            context.ProtocolMessage.SetParameter("audience", 
                                authConfiguration.MachineToMachineSettings.ApiIdentifier);
                        }
						if (context.Properties.Items.ContainsKey("goToRegisterPage"))
                        {
	                        context.ProtocolMessage.SetParameter("register", 
		                        context.Properties.Items["goToRegisterPage"]);
						}

						return Task.FromResult(0);
                    },
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                       var logoutUri = $"https://{authConfiguration.TenantDomain}/v2/logout?client_id={authConfiguration.WebSettings.ClientId}";
                       var postLogoutUri = context.Properties.RedirectUri;
                       if (!string.IsNullOrEmpty(postLogoutUri))
                       {
                           if (postLogoutUri.StartsWith("/"))
                           {
                               // transform to absolute
                               var request = context.Request;
                               postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                           }
                           logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                       }
                       context.Response.Redirect(logoutUri);
                       context.HandleResponse();
                       return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddAuthorisation(this IServiceCollection services, IAuthConfiguration authConfiguration, Action<AuthorizationOptions> authorizationOptions = null)
        {
            services.TryAddSingleton(authConfig => authConfiguration);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://{authConfiguration.TenantDomain}";
                    options.Audience = authConfiguration.MachineToMachineSettings.ApiIdentifier;
                });

            Action<AuthorizationOptions> defaultOptions = options => { };

#if NETSTANDARD
                services.AddAuthorization(authorizationOptions ?? defaultOptions);
#else
            	services.AddAuthorizationCore(authorizationOptions ?? defaultOptions);
#endif
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorisationPolicyProvider>();
			services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
        }
    }
}
#endif