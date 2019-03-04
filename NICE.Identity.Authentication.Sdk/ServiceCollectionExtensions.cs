using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Authentication;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.External;
using IAuthenticationService = NICE.Identity.Authentication.Sdk.Abstractions.IAuthenticationService;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace NICE.Identity.Authentication.Sdk
{
	public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationSdk(this IServiceCollection services,
                                                              IConfiguration configuration,
                                                              string authorisationServiceConfigurationPath,
															  bool supportM2M = false)
        {
            services.Configure<AuthorisationServiceConfiguration>(configuration.GetSection(authorisationServiceConfigurationPath));
            
            InstallAuthorisation(services);
	        InstallAuthenticationService(services, configuration);
			//var authenticationBuilder = InstallAuthenticationService(services, configuration);
			//if (supportM2M)
			//{
			// authenticationBuilder.InstallM2MAuthentication(services, configuration);

			//}

			return services;
        }

        private static void InstallAuthorisation(IServiceCollection services)
        {
            services.AddHttpClient<IHttpClientDecorator, HttpClientDecorator>();
            services.AddScoped<IAuthorisationService, AuthorisationApiService>();

            //services.AddAuthorization(options =>
            //{
            //    //TODO: Investigate Roles - options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrator"));
            //    options.AddPolicy(PolicyTypes.Administrator,
            //        policy => policy.Requirements.Add(new RoleRequirement($"{PolicyTypes.Administrator}")));
            //});

            //services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
        }

	    private static void InstallM2MAuthentication(this AuthenticationBuilder builder, IServiceCollection services, IConfiguration configuration)
	    {
			string domain = $"https://{configuration["Auth0:Domain"]}/";
		    builder
			    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			    {
				    options.Authority = domain;
				    options.Audience = configuration["Auth0:ApiIdentifier"];
			    });

			//services.AddAuthentication(options =>
			//{
			//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//}).AddJwtBearer(options =>
			//{
			//	options.Authority = domain;
			//	options.Audience = configuration["Auth0:ApiIdentifier"];
			//});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("read:messages",
					policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
			});

			// register the scope authorization handler
			services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
		}


	    private static AuthenticationBuilder InstallAuthenticationService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, Auth0Service>();
            

            string domain = $"https://{configuration["Auth0:Domain"]}/";

            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:messages",
                    policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

			var authenticationBuilder = services.AddAuthentication(options =>
			 {
				 options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				 options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				 options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			 }).AddCookie().AddJwtBearer(options =>
			 {
				 options.Authority = domain;
				 options.Audience = configuration["Auth0:ApiIdentifier"];
			 });


			//Add authentication services
			//var authenticationBuilder = services.AddAuthentication(options =>
			//		{
			//			 //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//			 //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//			 //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//			 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//		})
			//			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			//			{
			//				options.Authority = domain;
			//				options.Audience = configuration["Auth0:ApiIdentifier"];
			//			});
			//authenticationBuilder
	  //.AddCookie()

			authenticationBuilder
			.AddOpenIdConnect("Auth0", options => {
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
	        return authenticationBuilder;
        }
    }
}