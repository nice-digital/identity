using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth0.Owin;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OpenIdConnect;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.External;
using NICE.Identity.Authentication.Sdk.Redis;
using NICE.Identity.NETFramework.Authorisation;
using Owin;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class AppBuilderExtensions
	{
		public static ContainerBuilder AddContainer<T>(this ContainerBuilder builder, IConfigurationRoot configuration) where T : IComponent
        {
			builder.RegisterOptions();

			var authConfiguration = new AuthConfiguration
            {
                Domain = configuration.GetSection("Auth0")["Domain"],
                ClientId = configuration.GetSection("Auth0")["ClientId"],
                ClientSecret = configuration.GetSection("Auth0")["ClientSecret"],
                RedirectUri = configuration.GetSection("Auth0")["RedirectUri"],
                PostLogoutRedirectUri = configuration.GetSection("Auth0")["PostLogoutRedirectUri"],
                ApiIdentifier = configuration.GetSection("Auth0")["ApiIdentifier"]
            };

            var redisConfig = new RedisConfiguration
            {
                IpConfig = configuration.GetSection("RedisServiceConfiguration")["IpConfig"],
                Port = int.Parse(configuration.GetSection("RedisServiceConfiguration")["Port"])
            };

            var authorisationServiceConfiguration = new AuthorisationServiceConfiguration
            {
				BaseUrl = configuration.GetSection("AuthorisationServiceConfiguration")["BaseUrl"],
				DurationOfBreakInMinutes = 1,
				HandledEventsAllowedBeforeBreaking = 3
            };

            var auth0ServiceConfiguration = new Auth0ServiceConfiguration
            {
				ApiIdentifier = configuration.GetSection("Auth0")["ApiIdentifier"],
				Domain = configuration.GetSection("Auth0")["Domain"],
				ClientId = configuration.GetSection("Auth0")["ClientId"],
				ClientSecret = configuration.GetSection("Auth0")["ClientSecret"],
				HandledEventsAllowedBeforeBreaking = 3,
				DurationOfBreakInMinutes = 1,
				GrantType = configuration.GetSection("Auth0")["Grant_Type"]
            };

            builder.RegisterInstance<IAuth0Configuration>(authConfiguration);
            builder.RegisterInstance(redisConfig);
            builder.RegisterInstance(authorisationServiceConfiguration);
            builder.RegisterInstance(auth0ServiceConfiguration);
			builder.RegisterType<HttpClientDecorator>().As<IHttpClientDecorator>();
			builder.RegisterType<AuthorisationApiService>().As<IAuthorisationService>();

			builder.Register(ctx => new HttpClient {BaseAddress = new Uri(ctx.Resolve<AuthorisationServiceConfiguration>().Host)})
			       .Named<HttpClient>("AuthorisationApi")
			       .SingleInstance();

			builder.Register(ctx => new HttpClientDecorator(ctx.ResolveNamed<HttpClient>("AuthorisationApi")))
			       .InstancePerDependency();

            builder.RegisterControllers(typeof(T).Assembly);
			builder.RegisterModule<AutofacWebTypesModule>();

			return builder;
        }

        public static void AddAuthentication(this IAppBuilder app, string clientName, AuthConfiguration authConfiguration, RedisConfiguration redisConfiguration)
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


            var domain = $"https://{authConfiguration.Domain}/";
            var apiIdentifier = authConfiguration.ApiIdentifier;

            var keyResolver = new OpenIdConnectSigningKeyResolver(domain);
            app.UseJwtBearerAuthentication(
	            new JwtBearerAuthenticationOptions
	            {
		            AuthenticationMode = AuthenticationMode.Active,
		            TokenValidationParameters = new TokenValidationParameters()
		            {
			            ValidAudience = apiIdentifier,
			            ValidIssuer = domain,
			            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid)
		            }
	            });
           

            app.UseCookieAuthentication(options);

            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				AuthenticationType = "Auth0",

				Authority = $"https://{authConfiguration.Domain}",

				ClientId = authConfiguration.ClientId,
				ClientSecret = authConfiguration.ClientSecret,

				RedirectUri = authConfiguration.RedirectUri,
				PostLogoutRedirectUri = authConfiguration.PostLogoutRedirectUri,

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
							notification.ProtocolMessage.SetParameter("audience", authConfiguration.ApiIdentifier);
						}
						else if(notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
						{
							var logoutUri = $"https://{authConfiguration.Domain}/v2/logout?client_id={authConfiguration.ClientId}";

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

        public static void Configure<TOptions>(this ContainerBuilder builder, Action<TOptions> configureOptions) where TOptions : class
        {
	        if (builder == null)
	        {
		        throw new ArgumentNullException(nameof(builder));
	        }

	        if (configureOptions == null)
	        {
		        throw new ArgumentNullException(nameof(configureOptions));
	        }

	        builder.RegisterInstance(new ConfigureOptions<TOptions>(configureOptions))
	               .As<IConfigureOptions<TOptions>>()
	               .SingleInstance();
        }


        public static void RegisterConfigurationOptions<TOptions>(this ContainerBuilder builder, IConfiguration config) where TOptions : class, new()
        {
	        if (builder == null)
	        {
		        throw new ArgumentNullException(nameof(builder));
	        }

	        if (config == null)
	        {
		        throw new ArgumentNullException(nameof(config));
	        }

	        builder.RegisterInstance(new ConfigurationChangeTokenSource<TOptions>(config))
	               .As<IOptionsChangeTokenSource<TOptions>>()
	               .SingleInstance();

	        builder.RegisterInstance(new ConfigureFromConfigurationOptions<TOptions>(config))
	               .As<IConfigureOptions<TOptions>>()
	               .SingleInstance();

	        builder.RegisterType<OptionsManager<TOptions>>().InstancePerLifetimeScope();

        }

        public static void RegisterOptions(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(OptionsManager<>))
              .As(typeof(IOptions<>))
              .SingleInstance();

            builder.RegisterGeneric(typeof(OptionsMonitor<>))
              .As(typeof(IOptionsMonitor<>))
              .SingleInstance();

            //builder.RegisterGeneric(typeof(OptionsSnapshot<>))
            //  .As(typeof(IOptionsSnapshot<>))
            //  .InstancePerLifetimeScope();
        }
    }
}
