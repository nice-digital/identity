using System.Collections.Generic;
#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.Extensions.Configuration;
#endif

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IAuthConfiguration
	{
		string TenantDomain { get; }
		AuthConfiguration.WebSettingsType WebSettings { get; }
		AuthConfiguration.MachineToMachineSettingsType MachineToMachineSettings { get; }
		IEnumerable<string> RolesWithAccessToUserProfiles { get; }
		string LoginPath { get; }
		string LogoutPath { get; }
		AuthConfiguration.RedisConfig RedisConfiguration { get; }
		string GoogleTrackingId { get;  }
	}

	/// <summary>
	/// This is the master class for auth configuration. don't put any anywhere else.
	/// </summary>
	public class AuthConfiguration : IAuthConfiguration
	{
		public class RedisConfig
		{
			public RedisConfig(bool enabled, string connectionString)
			{
				Enabled = enabled;
				ConnectionString = connectionString;
			}

			public bool Enabled { get; set; }
			public string ConnectionString { get; set; }
		}

#if NETSTANDARD2_0 || NETCOREAPP3_1
		public AuthConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			TenantDomain = section["Domain"];
			WebSettings = new WebSettingsType(section["ClientId"], section["ClientSecret"], section["RedirectUri"], section["PostLogoutRedirectUri"], section["AuthorisationServiceUri"], section["CallBackPath"]);
			MachineToMachineSettings = new MachineToMachineSettingsType(section["ApiIdentifier"]);
			RolesWithAccessToUserProfiles = configuration.GetSection(appSettingsSectionName + ":RolesWithAccessToUserProfiles").Get<string[]>() ?? new string[0];
			LoginPath = section["LoginPath"];
			LogoutPath = section["LogoutPath"];
			GoogleTrackingId = section["GoogleTrackingId"];

			var redisSection = configuration.GetSection(appSettingsSectionName + ":RedisServiceConfiguration");
			RedisConfiguration = new RedisConfig(
				enabled: bool.TryParse(redisSection["Enabled"], out var enabled) ? enabled : false,
				connectionString: redisSection["ConnectionString"]
			);
		}
#endif
#if NETFRAMEWORK
		public AuthConfiguration(IdAMWebConfigSection webConfigSection)
		{
			TenantDomain = webConfigSection.Domain;
			WebSettings = new WebSettingsType(webConfigSection.ClientId, webConfigSection.ClientSecret, webConfigSection.RedirectUri, webConfigSection.PostLogoutRedirectUri, webConfigSection.AuthorisationServiceUri, webConfigSection.CallBackPath);
			MachineToMachineSettings = new MachineToMachineSettingsType(webConfigSection.ApiIdentifier);
			RolesWithAccessToUserProfiles = webConfigSection.RolesWithAccessToUserProfiles;

			LoginPath = webConfigSection.LoginPath;
			LogoutPath = webConfigSection.LogoutPath;
			GoogleTrackingId = webConfigSection.GoogleTrackingId;

			RedisConfiguration = new RedisConfig(
				enabled: webConfigSection.RedisEnabled,
				connectionString: webConfigSection.RedisConnectionString
			);
		}
#endif

		public AuthConfiguration(string tenantDomain, string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string apiIdentifier, string authorisationServiceUri, 
			string googleTrackingId, string grantType = null, string callBackPath = "/signin-auth0", IEnumerable<string> rolesWithAccessToUserProfiles = null, string loginPath = null, string logoutPath = null,
			bool redisEnabled = false, string redisConnectionString = null)
		{
			TenantDomain = tenantDomain;
			WebSettings = new WebSettingsType(clientId, clientSecret, redirectUri, postLogoutRedirectUri, authorisationServiceUri, callBackPath);
			MachineToMachineSettings = new MachineToMachineSettingsType(apiIdentifier);
			RolesWithAccessToUserProfiles = rolesWithAccessToUserProfiles ?? new List<string>();

			LoginPath = loginPath;
			LogoutPath = logoutPath;
			GoogleTrackingId = googleTrackingId;

			RedisConfiguration = new RedisConfig(
				enabled: redisEnabled,
				connectionString: redisConnectionString
			);
		}

		public string TenantDomain { get; }

		public class WebSettingsType
		{
			public WebSettingsType(string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string authorisationServiceUri, string callBackPath)
			{
				ClientId = clientId;
				ClientSecret = clientSecret;
				RedirectUri = redirectUri;
				PostLogoutRedirectUri = postLogoutRedirectUri;
				AuthorisationServiceUri = authorisationServiceUri;
				CallBackPath = callBackPath;
			}

			public string ClientId { get; set; }
			public string ClientSecret { get; set; }
			public string RedirectUri { get; set; }
			public string PostLogoutRedirectUri { get; set; }

			public string AuthorisationServiceUri { get; set; }
			public string CallBackPath { get; set; }
		}
		public WebSettingsType WebSettings { get; set; }


		public class MachineToMachineSettingsType
		{
			public MachineToMachineSettingsType(string apiIdentifier)
			{
				ApiIdentifier = apiIdentifier;
			}

			public string ApiIdentifier { get; set; }
		}
		public MachineToMachineSettingsType MachineToMachineSettings { get; set; }

		public IEnumerable<string> RolesWithAccessToUserProfiles { get; }

		private string _loginPath;
		public string LoginPath
		{
			get => _loginPath ?? "/account/login";
			set => _loginPath = value;
		}

		private string _logoutPath;
		public string LogoutPath
		{
			get => _logoutPath ?? "/account/logout";
			set => _logoutPath = value;
		}

		public RedisConfig RedisConfiguration { get; set; }
		public string GoogleTrackingId { get; set; }
	}
}