using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Authentication;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.External;
using StackExchange.Redis;
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

            return services;
	    }

	    public static IServiceCollection AddAuthenticationSdk(this IServiceCollection services,
                                                              IConfiguration configuration,
                                                              string authorisationServiceConfigurationPath)
        {
            services.Configure<AuthorisationServiceConfiguration>(configuration.GetSection(authorisationServiceConfigurationPath));
            services.Configure<Auth0ServiceConfiguration>(configuration.GetSection("Auth0"));
	        services.Configure<AuthConfiguration>(configuration.GetSection("Auth0"));
			services.AddSingleton<IHttpConfiguration, Auth0ServiceConfiguration>();
            services.AddScoped<IAuthenticationService, Auth0Service>();
            services.AddScoped<IAuth0Configuration, AuthConfiguration>();
            services.AddHttpClientWithHttpConfiguration<Auth0ServiceConfiguration>("Auth0ServiceApiClient");

            InstallAuthorisation(services);
            InstallAuthenticationService(services, configuration);

            return services;
        }

        private static void InstallAuthorisation(IServiceCollection services)
        {
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.AddScoped<IAuthorisationService, AuthorisationApiService>();

	        services.AddAuthorization(); 
			services.AddSingleton<IAuthorizationPolicyProvider, AuthorisationPolicyProvider>(); //policies added here.

			services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
        }

        private static void InstallAuthenticationService(IServiceCollection services, IConfiguration configuration)
        {
	        string domain = $"https://{configuration["Auth0:Domain"]}/";

	  //      services.AddAuthorization(options =>
	  //      {
		 //       options.AddPolicy("read:messages",
			//        policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));

		 //       options.AddPolicy("getusers",
			//        policy => policy.Requirements.Add(new HasScopeRequirement("getusers", domain)));
			//});

			services.AddScoped<IAuthenticationService, Auth0Service>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            // Add authentication services
            services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			}).AddCookie()
			.AddJwtBearer(options =>
			{
				options.Authority = domain;
				options.Audience = configuration["Auth0:ApiIdentifier"];
			}).AddOpenIdConnect("Auth0", options => 
			{
				// Set the authority to your Auth0 domain
				options.Authority = $"https://{configuration["Auth0:Domain"]}";

				// Configure the Auth0 Client ID and Client Secret
				options.ClientId = configuration["Auth0:ClientId"];
				options.ClientSecret = configuration["Auth0:ClientSecret"];

				// Set response type to code
				options.ResponseType = "code";

				// Configure the scope
				options.Scope.Clear();
				options.Scope.Add("openid read:profile");

				// Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
				// Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
				options.CallbackPath = new PathString("/signin-auth0");

				// Configure the Claims Issuer to be Auth0
				options.ClaimsIssuer = "Auth0";

				// Saves tokens to the AuthenticationProperties
				options.SaveTokens = true;

				options.Events = new OpenIdConnectEvents
				{
                    OnTokenValidated = (context) =>
                    {
                        return Task.CompletedTask;
                    },
					// handle the logout redirection 
					OnRedirectToIdentityProviderForSignOut = (context) =>
					{
						var logoutUri = $"https://{configuration["Auth0:Domain"]}/v2/logout?client_id={configuration["Auth0:ClientId"]}";

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

        public static T GetConfiguration<T>(this IServiceCollection collection) where T : class, new()
        {
	        var sp = collection.BuildServiceProvider();
	        return sp.GetService<IOptions<T>>().Value;
        }

        public static void AddHttpClientWithHttpConfiguration<T>(this IServiceCollection services, string name)
	        where T : class, IHttpConfiguration, new()
        {
	        if (services == null)
		        throw new ArgumentNullException(nameof(services));
	        if (name == null)
		        throw new ArgumentNullException(nameof(name));

	        IHttpConfiguration options = services.GetConfiguration<T>();

	        services.AddHttpClient(name, client =>
	                               {
		                               client.BaseAddress = new Uri(options.Host);
		                               client.Timeout = TimeSpan.FromMinutes(1);
		                               client.DefaultRequestHeaders.Authorization = options.AuthenticationHeaderValue;
	                               }).AddTransientHttpErrorPolicy(options.CircuitBreaker);
        }
    }
}