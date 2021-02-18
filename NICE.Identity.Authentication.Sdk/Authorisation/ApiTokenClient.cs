using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.TokenStore;
using Polly.Retry;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public interface IApiTokenClient
	{
        /// <summary>
        /// Use this overload to get an access token using the Application (clientid) in AuthConfiguration, from DI
        /// </summary>
        /// <returns></returns>
		Task<string> GetAccessToken(IAuthConfiguration authConfiguration);

        /// <summary>
        /// This overload is intended to get an access token from a different application, like the management api
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="apiIdentifier"></param>
        /// <returns></returns>
		Task<string> GetAccessToken(string domain, string clientId, string clientSecret, string apiIdentifier);
    }

	public class ApiTokenClient : IApiTokenClient
	{
		private readonly IAuthenticationConnection _authenticationConnection;
		private readonly IApiTokenStore _tokenStore;
		private readonly AsyncRetryPolicy _retryPolicy;

		/// <summary>
        /// This dictionary should contain the last token store key for each client id. the key is client id, the value is the token store key for redis (not the access token!)
        /// </summary>
		private static readonly ConcurrentDictionary<string, string> TokenStoreKeys = new ConcurrentDictionary<string, string>();

#if NETSTANDARD2_0 || NETCOREAPP3_1
        public ApiTokenClient(IApiTokenStore tokenStore, IAuthenticationConnection authenticationConnection)
		{
			_tokenStore = tokenStore;
			_authenticationConnection = authenticationConnection;
			_retryPolicy = APIConfiguration.GetAuth0RetryPolicy();
		}
#else
		/// <summary>
		/// This overload is for .net framework. The SDK doesn't populate the DI with IApiTokenStore AND IAuthenticationConnection like it does in ServiceCollectionExtensions. so instead we rely
		/// on just the AuthConfiguration and the http client. if the httpclient is null, then a new one will be created and disposed in each request (which isn't great).
		/// </summary>
		/// <param name="authConfiguration"></param>
		/// <param name="httpClient">optional, but you should pass it in and share it among connections, as long as the default behaviour isn't reconfigured.</param>
		public ApiTokenClient(IAuthConfiguration authConfiguration, HttpClient httpClient = null)
        {
	        _tokenStore = new RedisApiTokenStore(authConfiguration.RedisConfiguration.ConnectionString);
	        _authenticationConnection = new HttpClientAuthenticationConnection(httpClient); 
			_retryPolicy = APIConfiguration.GetAuth0RetryPolicy();
        }
#endif

        public async Task<string> GetAccessToken(IAuthConfiguration authConfiguration)
        {
	        return await GetAccessToken(authConfiguration.TenantDomain, 
										authConfiguration.WebSettings.ClientId,
										authConfiguration.WebSettings.ClientSecret, 
										authConfiguration.MachineToMachineSettings.ApiIdentifier);
        }


        public async Task<string> GetAccessToken(string domain, string clientId, string clientSecret, string apiIdentifier)
        {
	        JwtToken token = null;
            if (TokenStoreKeys.ContainsKey(clientId))
            {
                token = await _tokenStore.RetrieveAsync(TokenStoreKeys[clientId]); //attempts to retrieve token from redis, if it hasn't expired.
                if (token != null)
                {
	                return token.AccessToken;
                }
            }
            
            //hit auth0 for the token. this is uncached.
            var tokenFromIdentityProvider = await GetClientCredentialsAccessTokenUncached(domain, clientId, clientSecret, apiIdentifier);
            
            //now store the token in redis. get back a redis token store key.
            var newTokenStoreKeyForThisClientId = await _tokenStore.StoreAsync(tokenFromIdentityProvider);

            //store the token in our concurrent dictionary. replace the old value if it exists.
            TokenStoreKeys.AddOrUpdate(clientId, newTokenStoreKeyForThisClientId, (key, oldTokenStoreKey) => newTokenStoreKeyForThisClientId);

            return tokenFromIdentityProvider.AccessToken;
        }

        private async Task<JwtToken> GetClientCredentialsAccessTokenUncached(string domain, string clientId, string clientSecret, string apiIdentifier)
        {
	        try
	        {
		        using (var client = new AuthenticationApiClient(new Uri($"https://{domain}/"), _authenticationConnection))
		        {
			        var managementApiTokenRequest = new ClientCredentialsTokenRequest
			        {
				        ClientId = clientId,
				        ClientSecret = clientSecret,
				        Audience = apiIdentifier
                    };

			        var accessTokenResponse = await _retryPolicy.ExecuteAsync(() => client.GetTokenAsync(managementApiTokenRequest));
                    
			        return new JwtToken
			        {
				        AccessToken = accessTokenResponse.AccessToken, 
				        ExpiresIn = accessTokenResponse.ExpiresIn,
				        TokenType = accessTokenResponse.TokenType
			        };
		        }
	        }
	        catch (Exception e)
	        {
		        throw new Exception("Error in GetAccessTokenForManagementAPI when calling the Management API.", e);
	        }
        }
	}
}
