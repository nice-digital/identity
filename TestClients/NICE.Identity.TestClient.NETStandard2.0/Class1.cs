using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json.Linq;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.TestClient.NETStandard2._0
{
	public class StandardTester
	{
		public void Go()
		{

			IServiceCollection services = new ServiceCollection();

			var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var secretsPath = Path.Combine(appDataPath, @"Microsoft\UserSecrets\b69bc28e-14c9-4c24-bd25-232e24a55745\secrets.json");
			var secretsFile = JObject.Parse(File.ReadAllText(secretsPath));

			var redisConfig = secretsFile.SelectToken("WebAppConfiguration.RedisServiceConfiguration");

			var authConfiguration = new AuthConfiguration(
				tenantDomain: secretsFile.SelectToken("WebAppConfiguration")["Domain"].ToString(),
				clientId: secretsFile.SelectToken("WebAppConfiguration")["ClientId"].ToString(),
				clientSecret: secretsFile.SelectToken("WebAppConfiguration")["ClientSecret"].ToString(),
				redirectUri: secretsFile.SelectToken("WebAppConfiguration")["RedirectUri"].ToString(),
				postLogoutRedirectUri: secretsFile.SelectToken("WebAppConfiguration")["PostLogoutRedirectUri"].ToString(),
				apiIdentifier: secretsFile.SelectToken("WebAppConfiguration")["ApiIdentifier"].ToString(),
				authorisationServiceUri: secretsFile.SelectToken("WebAppConfiguration")["AuthorisationServiceUri"].ToString(),
				redisEnabled: bool.Parse(redisConfig["Enabled"].ToString()),
				redisConnectionString: redisConfig["ConnectionString"].ToString(),
				googleTrackingId: secretsFile.SelectToken("WebAppConfiguration")["GoogleTrackingId"].ToString()
			);
			
			services.AddAuthentication(authConfiguration);
			services.AddAuthorisation(authConfiguration);
		}

	}
}
