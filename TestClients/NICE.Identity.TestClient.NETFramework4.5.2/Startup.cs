using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.TestClient.NETFramework452;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace NICE.Identity.TestClient.NETFramework452
{
	public class Startup
	{

		public void Configuration(IAppBuilder app)
		{
			//here's an example of how to get configuration from a web.config file in a single section:
			var authConfigurationWebConfig = new AuthConfiguration(IdAMWebConfigSection.GetConfig());


			//alternatively you could get each property from the webconfig's appsetting's section, just like the redirecturi and postLogoutRedirectUri below.
			//(all other fields below come from the secrets file in .net core project, just because this repo is public)

			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var secretsPath = Path.Combine(appDataPath, @"Microsoft\UserSecrets\b69bc28e-14c9-4c24-bd25-232e24a55745\secrets.json");
			var secretsFile = JObject.Parse(File.ReadAllText(secretsPath));

			var redisConfig = secretsFile.SelectToken("WebAppConfiguration.RedisServiceConfiguration");

			var authConfiguration = new AuthConfiguration(
				tenantDomain: secretsFile.SelectToken("WebAppConfiguration")["Domain"].ToString(),
				clientId: secretsFile.SelectToken("WebAppConfiguration")["ClientId"].ToString(),
				clientSecret: secretsFile.SelectToken("WebAppConfiguration")["ClientSecret"].ToString(),
				redirectUri: ConfigurationManager.AppSettings["auth0:RedirectUri"],
				postLogoutRedirectUri: ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"],
				apiIdentifier: secretsFile.SelectToken("WebAppConfiguration")["ApiIdentifier"].ToString(),
                authorisationServiceUri: secretsFile.SelectToken("WebAppConfiguration")["AuthorisationServiceUri"].ToString(),
				googleTrackingId: secretsFile.SelectToken("WebAppConfiguration")["GoogleTrackingId"].ToString(),
				redisEnabled: bool.Parse(redisConfig["Enabled"].ToString()),
				redisConnectionString: redisConfig["ConnectionString"].ToString()
			);
			
			//AutoFAC DI
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterInstance(new ApiTokenClient(authConfiguration)); //authconfig added to DI here.
			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


			//show more detail on some auth errors. only set to true for dev/debug.
			IdentityModelEventSource.ShowPII = true; 

			
			app.AddOwinAuthentication(authConfiguration); 
		}
	}
}
