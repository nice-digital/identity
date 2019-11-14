using Microsoft.AspNetCore.Authentication.Cookies;
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
using NICE.Identity.Authentication.Sdk.External;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
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
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.AddSingleton(authConfig => authConfiguration);
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAPIService, APIService>();

			// Add authentication services
			services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
            .AddCookie(options => options.Cookie.Name = AuthenticationConstants.CookieName)
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
						var uri = new Uri($"{authConfiguration.WebSettings.AuthorisationServiceUri}{Constants.AuthorisationURLs.GetClaims}{userId}");
						var client = httpClient ?? new HttpClient();

						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
						var responseMessage = await client.GetAsync(uri); //call the api to get all the claims for the current user
						if (responseMessage.IsSuccessStatusCode)
						{
							var allClaims = JsonConvert.DeserializeObject<Claim[]>(await responseMessage.Content.ReadAsStringAsync());
							//add in all the claims from retrieved from the api, excluding roles where the host doesn't match the current.
							var claimsToAdd = allClaims.Where(claim => (!claim.Type.Equals(ClaimType.Role)) ||
							                                           (claim.Type.Equals(ClaimType.Role) &&
																	    claim.Issuer.Equals(context.HttpContext.Request.Host.Host, StringComparison.OrdinalIgnoreCase)))
														.Select(claim => new System.Security.Claims.Claim(claim.Type, claim.Value, null, claim.Issuer)).ToList();
							
							if (claimsToAdd.Any())
							{
								context.Principal.AddIdentity(new ClaimsIdentity(claimsToAdd));
							}
						}
						else
						{
							throw new Exception("Error trying to set claims when signing in."); 
						}
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
                    },
                    //,OnRemoteFailure = (context) =>
					//{
					//	context.Response.Redirect("/"); 
					//	context.HandleResponse();
					//	return Task.FromResult(0);
					//}
                };
            });
        }

        public static void AddAuthorisation(this IServiceCollection services, IAuthConfiguration authConfiguration)
        {
            // TODO: refactor HttpClientDecorator and rename authConfiguration
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.TryAddSingleton(authConfig => authConfiguration);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://{authConfiguration.TenantDomain}";
                    options.Audience = authConfiguration.MachineToMachineSettings.ApiIdentifier;
                });
            services.AddAuthorization();

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorisationPolicyProvider>();

            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ScopeRequirementHandler>();
        }
    }
}