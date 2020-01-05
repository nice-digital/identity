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
	}

	/// <summary>
	/// This is the master class for auth configuration. don't put any anywhere else.
	/// </summary>
	public class AuthConfiguration : IAuthConfiguration
	{
#if NETSTANDARD2_0 || NETCOREAPP3_1
		public AuthConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			TenantDomain = section["Domain"];
			WebSettings = new WebSettingsType(section["ClientId"], section["ClientSecret"], section["RedirectUri"], section["PostLogoutRedirectUri"], section["AuthorisationServiceUri"], section["CallBackPath"]);
			MachineToMachineSettings = new MachineToMachineSettingsType(section["ApiIdentifier"]);
			RolesWithAccessToUserProfiles = configuration.GetSection(appSettingsSectionName + ":RolesWithAccessToUserProfiles").Get<string[]>() ?? new string[0];
		}
#endif
		public AuthConfiguration(string tenantDomain, string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string apiIdentifier, string authorisationServiceUri, string grantType = null, string callBackPath = "/signin-auth0", IEnumerable<string> rolesWithAccessToUserProfiles = null)
		{
			TenantDomain = tenantDomain;
			WebSettings = new WebSettingsType(clientId, clientSecret, redirectUri, postLogoutRedirectUri, authorisationServiceUri, callBackPath);
			MachineToMachineSettings = new MachineToMachineSettingsType(apiIdentifier);
			RolesWithAccessToUserProfiles = rolesWithAccessToUserProfiles ?? new List<string>();
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
	}
}