using Auth0.AuthenticationApi;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.TokenStore;

namespace NICE.Identity.Test.Infrastructure
{
	public class StubApiTokenClient : ApiTokenClient
	{
		public StubApiTokenClient(IAuthConfiguration authConfiguration, IApiTokenStore tokenStore, IAuthenticationConnection authenticationConnection) 
			: base(authConfiguration, tokenStore, authenticationConnection)
		{
		}
		
		/// <summary>
		/// The whole purpose for this class
		/// </summary>
		public static void ClearTokenStore()
		{
			TokenStoreKeys.Clear();
		}
	}
}
