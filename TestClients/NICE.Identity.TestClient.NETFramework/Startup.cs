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

			// Configure Auth0 parameters
			string auth0Domain = secretsFile.SelectToken("Auth0")["Domain"].ToString();
			string auth0ClientId = secretsFile.SelectToken("Auth0")["ClientId"].ToString();
			string auth0ClientSecret = secretsFile.SelectToken("Auth0")["ClientSecret"].ToString();
			string auth0RedirectUri = secretsFile.SelectToken("Auth0")["RedirectUri"].ToString();
			string auth0PostLogoutRedirectUri = secretsFile.SelectToken("Auth0")["PostLogoutRedirectUri"].ToString();

			app.AddAuthentication(auth0Domain, auth0ClientId, auth0ClientSecret, auth0RedirectUri, auth0PostLogoutRedirectUri);
			
		}
	}
}
