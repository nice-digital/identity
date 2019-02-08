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

			// Enable Kentor Cookie Saver middleware
			app.UseKentorOwinCookieSaver();

			// Set Cookies as default authentication type
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
				LoginPath = new PathString("/Account/Login")
			});

			// Configure Auth0 authentication
			app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
			{
				AuthenticationType = "Auth0",

				Authority = $"https://{auth0Domain}",

				ClientId = auth0ClientId,
				ClientSecret = auth0ClientSecret,

				RedirectUri = auth0RedirectUri,
				PostLogoutRedirectUri = auth0PostLogoutRedirectUri,

				ResponseType = OpenIdConnectResponseType.CodeIdToken,
				Scope = "openid profile",

				TokenValidationParameters = new TokenValidationParameters
				{
					NameClaimType = "name"
				},

				Notifications = new OpenIdConnectAuthenticationNotifications
				{
					RedirectToIdentityProvider = notification =>
					{
						if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
						{
							var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

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
	}
}
