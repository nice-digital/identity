﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using AuthenticationService = NICE.Identity.Authentication.Sdk.Authentication.AuthenticationService;
using Claim = NICE.Identity.Authentication.Sdk.Domain.Claim;
using IAuthenticationService = NICE.Identity.Authentication.Sdk.Authentication.IAuthenticationService;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCacheSDK(this IServiceCollection services,
                                                              IConfiguration configuration,
                                                              string redisCacheServiceConfigurationPath,
                                                              string clientName)
        {
            services.Configure<RedisConfiguration>(configuration.GetSection(redisCacheServiceConfigurationPath));
            var serviceProvider = services.BuildServiceProvider();
            var redisConfiguration = serviceProvider.GetService<IOptions<RedisConfiguration>>().Value;

            if (redisConfiguration.Enabled)
            {
                var redis = ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString);

                services.AddDataProtection()
                    .SetApplicationName(clientName)
                    .PersistKeysToStackExchangeRedis(redis, $"{Guid.NewGuid().ToString()}.Id-Keys");

                services.AddSession(options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.Name = $"{clientName}.Session";
                    options.Cookie.HttpOnly = true;
                    options.IdleTimeout = TimeSpan.FromMinutes(10);
                });
            }

            return services;
        }

        public static void AddAuthentication(this IServiceCollection services, IAuthConfiguration authConfiguration, HttpClient httpClient = null)
        {
            services.AddSingleton(authConfig => authConfiguration);
			services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.TryAddScoped<IAPIService, APIService>();
            services.AddHttpContextAccessor();
	        services.AddHttpClient();
            
			// Add authentication services
			services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
            .AddCookie(options =>
            {
	            options.Cookie.Name = AuthenticationConstants.CookieName;

				options.Events = new CookieAuthenticationEvents
				{
					OnValidatePrincipal = context =>
					{
						//check to see if user is authenticated first
						if (context.Principal.Identity.IsAuthenticated)
						{
							//get the users tokens
							var tokens = context.Properties.GetTokens();
							var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
							var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
							var exp = tokens.FirstOrDefault(t => t.Name == "expires_at");
							var expires = DateTime.Parse(exp.Value);
							//check to see if the token has expired
							if (expires < DateTime.Now)
							{

								//todo: use refresh token here.

								//token is expired, let's attempt to renew
								//var tokenEndpoint = "https://token.endpoint.server";
								//var tokenClient = new TokenClient(tokenEndpoint, clientId, clientSecret);
								//var tokenResponse = tokenClient.RequestRefreshTokenAsync(refreshToken.Value).Result;
								//check for error while renewing - any error will trigger a new login.
								//if (tokenResponse.IsError)
								//{
								//	//reject Principal
									context.RejectPrincipal();
								//	return Task.CompletedTask;
								//}
								////set new token values
								//refreshToken.Value = tokenResponse.RefreshToken;
								//accessToken.Value = tokenResponse.AccessToken;
								////set new expiration date
								//var newExpires = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
								//exp.Value = newExpires.ToString("o", CultureInfo.InvariantCulture);
								////set tokens in auth properties 
								//context.Properties.StoreTokens(tokens);
								////trigger context to renew cookie with new token values
								//context.ShouldRenew = true;
								return Task.CompletedTask;
							}
						}
						return Task.CompletedTask;
					}
				};

			})
            .AddOpenIdConnect(AuthenticationConstants.AuthenticationScheme, options => {
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
                    NameClaimType = "name"
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
						var client = httpClient ?? new HttpClient();
						await ClaimsHelper.AddClaimsToUser(authConfiguration, userId, accessToken, context.HttpContext.Request.Host.Host, context.Principal, client);
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
			services.AddAuthorization(authorizationOptions ?? defaultOptions);
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorisationPolicyProvider>();
			services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ScopeRequirementHandler>();
        }
    }
}