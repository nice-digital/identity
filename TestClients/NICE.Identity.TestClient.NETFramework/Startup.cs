using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
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

			var authConfiguration = new AuthConfiguration
			{
				Domain = secretsFile.SelectToken("Auth0")["Domain"].ToString(),
				ClientId = secretsFile.SelectToken("Auth0")["ClientId"].ToString(),
				ClientSecret = secretsFile.SelectToken("Auth0")["ClientSecret"].ToString(),
				RedirectUri = secretsFile.SelectToken("Auth0")["RedirectUri"].ToString(),
				PostLogoutRedirectUri = secretsFile.SelectToken("Auth0")["PostLogoutRedirectUri"].ToString()
			};

			app.AddAuthentication(authConfiguration);
			
		}
	}
}
