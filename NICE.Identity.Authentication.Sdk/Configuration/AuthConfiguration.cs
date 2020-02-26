using System.Collections.Generic;

#if NET461 || NETSTANDARD2_0 || NETCOREAPP
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

			public bool Enabled { get; private set; }
			public string ConnectionString { get; private set; }
		}

#if NET461 || NETSTANDARD2_0 || NETCOREAPP
		public AuthConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			TenantDomain = section["Domain"];
			WebSettings = new WebSettingsType(section["ClientId"], section["ClientSecret"], section["RedirectUri"], section["PostLogoutRedirectUri"], section["AuthorisationServiceUri"], section["CallBackPath"]);
			MachineToMachineSettings = new MachineToMachineSettingsType(section["ApiIdentifier"]);

			var rolesSection = configuration.GetSection(appSettingsSectionName + ":RolesWithAccessToUserProfiles");

#if NET461
			RolesWithAccessToUserProfiles = new List<string> {"todo! "}; //todo!
#else
			RolesWithAccessToUserProfiles = rolesSection.Get<string[]>() ?? new string[0];
#endif

			LoginPath = section["LoginPath"];
			LogoutPath = section["LogoutPath"];

			var redisSection = configuration.GetSection(appSettingsSectionName + ":RedisServiceConfiguration");
			RedisConfiguration = new RedisConfig(
				enabled: bool.TryParse(redisSection["Enabled"], out var enabled) ? enabled : false,
				connectionString: redisSection["ConnectionString"]
			);
		}
#endif
			public AuthConfiguration(string tenantDomain, string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string apiIdentifier, string authorisationServiceUri, 
			string grantType = null, string callBackPath = "/signin-auth0", IEnumerable<string> rolesWithAccessToUserProfiles = null, string loginPath = null, string logoutPath = null,
			bool redisEnabled = false, string redisConnectionString = null)
		{
			TenantDomain = tenantDomain;
			WebSettings = new WebSettingsType(clientId, clientSecret, redirectUri, postLogoutRedirectUri, authorisationServiceUri, callBackPath);
			MachineToMachineSettings = new MachineToMachineSettingsType(apiIdentifier);
			RolesWithAccessToUserProfiles = rolesWithAccessToUserProfiles ?? new List<string>();

			LoginPath = loginPath;
			LogoutPath = logoutPath;

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

			public string ClientId { get; private set; }
			public string ClientSecret { get; private set; }
			public string RedirectUri { get; private set; }
			public string PostLogoutRedirectUri { get; private set; }

			public string AuthorisationServiceUri { get; private set; }
			public string CallBackPath { get; private set; }
		}
		public WebSettingsType WebSettings { get; private set; }


		public class MachineToMachineSettingsType
		{
			public MachineToMachineSettingsType(string apiIdentifier)
			{
				ApiIdentifier = apiIdentifier;
			}

			public string ApiIdentifier { get; private set; }
		}
		public MachineToMachineSettingsType MachineToMachineSettings { get; private set; }

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

		public RedisConfig RedisConfiguration { get; private set; }
	}
}