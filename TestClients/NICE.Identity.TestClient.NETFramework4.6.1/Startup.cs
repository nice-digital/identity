using System;
using System.Configuration;
using System.IO;
using Microsoft.IdentityModel.Logging;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;
using Owin;

[assembly: OwinStartup(typeof(NICE.Identity.TestClient.NETFramework.Startup))]

namespace NICE.Identity.TestClient.NETFramework
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var secretsPath = Path.Combine(appDataPath, @"Microsoft\UserSecrets\b69bc28e-14c9-4c24-bd25-232e24a55745\secrets.json");
			var secretsFile = JObject.Parse(File.ReadAllText(secretsPath));

			var authConfiguration = new AuthConfiguration(
				tenantDomain: secretsFile.SelectToken("WebAppConfiguration")["Domain"].ToString(),
				clientId: secretsFile.SelectToken("WebAppConfiguration")["ClientId"].ToString(),
				clientSecret: secretsFile.SelectToken("WebAppConfiguration")["ClientSecret"].ToString(),
				redirectUri: ConfigurationManager.AppSettings["auth0:RedirectUri"],
				postLogoutRedirectUri: ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"], 
				apiIdentifier: secretsFile.SelectToken("WebAppConfiguration")["ApiIdentifier"].ToString(),
                authorisationServiceUri: secretsFile.SelectToken("WebAppConfiguration")["AuthorisationServiceUri"].ToString()
				);

			IdentityModelEventSource.ShowPII = true; //show more detail on some auth errors. only set to true for dev/debug.

			//var redisConfig = new RedisConfiguration
			//{
			//	IpConfig = secretsFile.SelectToken("RedisServiceConfiguration")["IpConfig"].ToString(),
			//	Port = int.Parse(secretsFile.SelectToken("RedisServiceConfiguration")["Port"].ToString()),
			//	Enabled = bool.Parse(secretsFile.SelectToken("RedisServiceConfiguration")["Enabled"].ToString())
			//};

			app.AddOwinAuthentication(authConfiguration);  //, redisConfig);

			

		}
	}
}
