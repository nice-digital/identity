﻿using System;
using System.Linq;
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

        public static void AddAuthentication(this IServiceCollection services, IAuthConfiguration authConfiguration)
        {
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.AddSingleton(authConfig => authConfiguration);
            services.AddScoped<IAuthenticationService, Auth0Service>();

            // Add authentication services
            services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
            .AddCookie()
            .AddOpenIdConnect("Auth0", options => {
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
                // Set the callback path, so Auth0 will call back to http://URI/signin-auth0
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

        public static void AddAuthorisation(this IServiceCollection services, IAuthConfiguration authConfiguration)
        {
            // TODO: refactor HttpClientDecorator an rename authConfiguration
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.AddScoped<IAuthorisationService, AuthorisationApiService>();
            services.AddSingleton(authConfig => authConfiguration);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://{authConfiguration.TenantDomain}";
                    options.Audience = authConfiguration.MachineToMachineSettings.ApiIdentifier;
                });
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorisationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
        }
    }
}