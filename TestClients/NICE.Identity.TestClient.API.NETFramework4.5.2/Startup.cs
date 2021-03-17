using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;
using Owin;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using NICE.Identity.Authentication.Sdk.Authorisation;

[assembly: OwinStartup(typeof(NICE.Identity.TestClient.API.NETFramework4._5._2.Startup))]

namespace NICE.Identity.TestClient.API.NETFramework4._5._2
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{

			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;


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

			////AutoFAC DI
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(WebApiApplication).Assembly);

			builder.RegisterInstance<IAuthConfiguration>(authConfiguration).SingleInstance();

			builder.Register(c => new HttpClient()).As<HttpClient>().SingleInstance();

			builder.RegisterInstance<IApiTokenClient>(new ApiTokenClient(authConfiguration)).SingleInstance();

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


			//show more detail on some auth errors. only set to true for dev/debug.
			IdentityModelEventSource.ShowPII = true;

			//login auth
			app.AddOwinAuthentication(authConfiguration);

			//api auth
			app.AddOwinAuthenticationForAPI(authConfiguration); //it's quite nasty for a website to support multiple auth - i.e. apis and user endpoints. but indev, publications and niceorg all do. 
		}
	}
}
