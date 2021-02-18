#if NETFRAMEWORK
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	/// <summary>
	/// This class is here for .NET Framework projects. In .NET Framework the configuration is stored in the web.config.
	///
	/// If you like you could store each of the below in a separate appsetting variable. Then instantiate the AuthConfiguration object using the constructor
	/// setting all the properties.
	///
	/// This class is here so instead of that, you can have the following in your web.config (without the CDATA):
	///<![CDATA[
	///	<configSections>
	///		<sectionGroup name = "NICE" >
	///			<section name="IdAM" type="NICE.Identity.Authentication.Sdk.Configuration.IdAMWebConfigSection, NICE.Identity.Authentication.Sdk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
	///		</sectionGroup>
	///	</configSections>
	///	<NICE>
	///		<IdAM ApiIdentifier = "1"
	///			ClientId="2"
	///			ClientSecret="3"
	///			AuthorisationServiceUri="4"
	///			Domain="5"
	///			PostLogoutRedirectUri="6"
	///			RedirectUri="7"
	///			GoogleTrackingId="8"
	///			RedisEnabled="True"
	///			RedisConnectionString="9"/>
	///	</NICE>
	/// ]]>
	///
	///that way in your global.asax.cs / owin startup.cs, when instantiating the AuthConfiguration object, you can just do:
	///
	/// var authConfiguration = new AuthConfiguration(IdAMWebConfigSection.GetConfig());
	///
	/// then add that object to your DI.
	/// </summary>
	public class IdAMWebConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("ApiIdentifier", IsRequired = true)]
		public string ApiIdentifier
		{
			get { return (string) this["ApiIdentifier"]; }
			set { this["ApiIdentifier"] = value; }
		}

		[ConfigurationProperty("ClientId", IsRequired = true)]
		public string ClientId
		{
			get { return (string)this["ClientId"]; }
			set { this["ClientId"] = value; }
		}

		[ConfigurationProperty("ClientSecret", IsRequired = true)]
		public string ClientSecret
		{
			get { return (string)this["ClientSecret"]; }
			set { this["ClientSecret"] = value; }
		}

		[ConfigurationProperty("AuthorisationServiceUri", IsRequired = true)]
		public string AuthorisationServiceUri
		{
			get { return (string)this["AuthorisationServiceUri"]; }
			set { this["AuthorisationServiceUri"] = value; }
		}

		[ConfigurationProperty("Domain", IsRequired = true)]
		public string Domain
		{
			get { return (string)this["Domain"]; }
			set { this["Domain"] = value; }
		}

		[ConfigurationProperty("PostLogoutRedirectUri", IsRequired = true)]
		public string PostLogoutRedirectUri
		{
			get { return (string)this["PostLogoutRedirectUri"]; }
			set { this["PostLogoutRedirectUri"] = value; }
		}

		[ConfigurationProperty("RedirectUri", IsRequired = true)]
		public string RedirectUri
		{
			get { return (string)this["RedirectUri"]; }
			set { this["RedirectUri"] = value; }
		}

		[ConfigurationProperty("GoogleTrackingId", IsRequired = true)]
		public string GoogleTrackingId
		{
			get { return (string)this["GoogleTrackingId"]; }
			set { this["GoogleTrackingId"] = value; }
		}

		[ConfigurationProperty("RedisEnabled", IsRequired = true)]
		public bool RedisEnabled
		{
			get { return bool.Parse((string)this["RedisEnabled"]); }
			set { this["RedisEnabled"] = value; }
		}

		[ConfigurationProperty("RedisConnectionString", IsRequired = true)]
		public string RedisConnectionString
		{
			get { return (string)this["RedisConnectionString"]; }
			set { this["RedisConnectionString"] = value; }
		}

		private string _defaultCallBackPath = "/signin-auth0";
		[ConfigurationProperty("CallBackPath", IsRequired = false)]
		public string CallBackPath
		{
			get { return (string)this["CallBackPath"] ?? _defaultCallBackPath; }
			set { this["CallBackPath"] = value; }
		}

		[ConfigurationProperty("RolesWithAccessToUserProfilesCSV", IsRequired = false)]
		public string RolesWithAccessToUserProfilesCSV
		{
			get { return (string)this["RolesWithAccessToUserProfilesCSV"] ?? string.Empty; }
			set { this["RolesWithAccessToUserProfilesCSV"] = value; }
		}

		public IEnumerable<string> RolesWithAccessToUserProfiles
		{
			get
			{
				return string.IsNullOrEmpty(RolesWithAccessToUserProfilesCSV) ? new List<string>() : RolesWithAccessToUserProfilesCSV.Split(',').Select(x => x.Trim()).ToList();
			}
		}

		[ConfigurationProperty("LoginPath", IsRequired = false)]
		public string LoginPath
		{
			get
			{
				var loginPath = (string)this["LoginPath"];
				return string.IsNullOrEmpty(loginPath) ? null : loginPath; //null is fine as there's a default in AuthConfiguration for this.
			}
			set { this["LoginPath"] = value; }
		}

		[ConfigurationProperty("LogoutPath", IsRequired = false)]
		public string LogoutPath
		{
			get
			{
				var logoutPath = (string)this["LogoutPath"];
				return string.IsNullOrEmpty(logoutPath) ? null : logoutPath; //null is fine as there's a default in AuthConfiguration for this.
			}
			set { this["LogoutPath"] = value; }
		}

		public static IdAMWebConfigSection GetConfig()
		{
			return ConfigurationManager.GetSection("NICE/IdAM") as IdAMWebConfigSection;
		}
	}
}
#endif