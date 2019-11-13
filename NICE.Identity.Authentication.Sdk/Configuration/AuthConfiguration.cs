using Microsoft.Extensions.Configuration;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IAuthConfiguration
	{
		string TenantDomain { get; }
		( string ClientId, string ClientSecret, string RedirectUri, string PostLogoutRedirectUri, string AuthorisationServiceUri, string CallBackPath ) WebSettings { get; set; }
		(string ApiIdentifier, string GrantType) MachineToMachineSettings { get; }
		string GrantTypeForMachineToMachine { get; }
	}

	/// <summary>
	/// This is the master class for auth configuration. don't put any anywhere else.
	/// </summary>
	public class AuthConfiguration : IAuthConfiguration
	{
		public AuthConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			TenantDomain = section["Domain"];
			WebSettings = (section["ClientId"], section["ClientSecret"], section["RedirectUri"], section["PostLogoutRedirectUri"], section["AuthorisationServiceUri"], section["CallBackPath"]);
			MachineToMachineSettings = (section["ApiIdentifier"], GrantTypeForMachineToMachine);
		}
		public AuthConfiguration(string tenantDomain, string clientId, string clientSecret, string redirectUri, string postLogoutRedirectUri, string apiIdentifier, string authorisationServiceUri, string grantType = null, string callBackPath = "/signin-auth0")
		{
			TenantDomain = tenantDomain;
			WebSettings = (clientId, clientSecret, redirectUri, postLogoutRedirectUri, authorisationServiceUri, callBackPath);
			MachineToMachineSettings = (apiIdentifier, grantType ?? GrantTypeForMachineToMachine);
		}

		public string TenantDomain { get; }

		public (
			string ClientId, 
			string ClientSecret,
			string RedirectUri,
			string PostLogoutRedirectUri,
            string AuthorisationServiceUri,
			string CallBackPath
			) 
			WebSettings { get; set; }
		
		public (
			string ApiIdentifier, 
			string GrantType
			) 
			MachineToMachineSettings { get; }

		public string GrantTypeForMachineToMachine => "client_credentials";
	}
}