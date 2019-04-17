using Microsoft.Extensions.Configuration;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IAuthConfiguration
	{
		string TenantDomain { get; }
		( string ClientId, string ClientSecret, string RedirectUri, string PostLogoutRedirectUri ) WebSettings { get; }
		(string ApiIdentifier, string GrantType) MachineToMachineSettings { get; }
	}

	/// <summary>
	/// This is the master class for auth configuration. don't put any anywhere else.
	/// </summary>
	public class AuthConfiguration : IAuthConfiguration
	{
		private const string GrantTypeForMachineToMachine = "client_credentials";
		public AuthConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			TenantDomain = section["Domain"];
			WebSettings = (section["ClientId"], section["ClientSecret"], section["RedirectUri"], section["PostLogoutRedirectUri"]);
			MachineToMachineSettings = (section["ApiIdentifier"], GrantTypeForMachineToMachine);
		}
		public AuthConfiguration(string tenantDomain, string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string apiIdentifier, string grantType = GrantTypeForMachineToMachine)
		{
			TenantDomain = tenantDomain;
			WebSettings = (clientId, clientSecret, redirectUri, postLogoutRedirectUri);
			MachineToMachineSettings = (apiIdentifier, GrantTypeForMachineToMachine);
		}

		public string TenantDomain { get; }

		public (
			string ClientId, 
			string ClientSecret,
			string RedirectUri,
			string PostLogoutRedirectUri
			) 
			WebSettings { get; }
		
		public (
			string ApiIdentifier, 
			string GrantType
			) 
			MachineToMachineSettings { get; }
	}
	
}