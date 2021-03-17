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
		/// Gets a machine to machine token a.k.a JWT token a.k.a. Bearer token a.k.a client_credentials grant/flow access token
		///
		/// Then stores it in (redis) cache. The token is stored in redis until it expires. The default duration for this is 24 hours, but it can be configured to anything in hostedpages octo.
		/// 
		/// This version relies on the AuthConfiguration passed in, in the constructor (likely in DI).
		/// </summary>
		/// <returns></returns>
		Task<string> GetAccessToken();

		/// <summary>
		/// Use this overload to get a cached access token using the Application (clientid) in AuthConfiguration, passed in.
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

	/// <summary>
	/// Provides a method (with overloads) to get an access token, and cache it in redis. Redis is mandatory for using this functionality.
	///
	/// In .net standard 2 / .net core 3.1, this interface is only added to DI when redis is enabled.
	/// 
	/// In .net framework where we don't add to DI, instantiating this class without redis enabled will throw an error at runtime.
	/// </summary>
	public class ApiTokenClient : IApiTokenClient
	{
		private readonly IAuthConfiguration _authConfiguration;
		private readonly IAuthenticationConnection _authenticationConnection;
		private readonly IApiTokenStore _tokenStore;
		private readonly AsyncRetryPolicy _retryPolicy;

		/// <summary>
        /// This dictionary should contain the last token store key for each client id. the key is client id, the value is the token store key for redis (not the access token!)
        ///
        /// The reason it's a dictionary is that some applications might need to communicate with more than one API. e.g. guidance-web hits APIs in: indev, publications, pathways + orchard
        ///
        /// static, so it's shared globally
        /// </summary>
		protected static ConcurrentDictionary<string, string> TokenStoreKeys { get; set; } = new ConcurrentDictionary<string, string>();

#if NETSTANDARD2_0 || NETCOREAPP3_1
        public ApiTokenClient(IAuthConfiguration authConfiguration, IApiTokenStore tokenStore, IAuthenticationConnection authenticationConnection)
		{
			_authConfiguration = authConfiguration;
			_tokenStore = tokenStore;
			_authenticationConnection = authenticationConnection;
			_retryPolicy = APIConfiguration.GetAuth0RetryPolicy();
		}
#elif NETFRAMEWORK
		/// <summary>
		/// This overload is for .net framework. The SDK doesn't populate the DI with IApiTokenStore AND IAuthenticationConnection like it does in ServiceCollectionExtensions. so instead we rely
		/// on just the AuthConfiguration and the http client. if the httpclient is null, then a new one will be created and disposed in each request (which isn't great).
		/// </summary>
		/// <param name="authConfiguration"></param>
		/// <param name="httpClient">optional, but you should pass it in and share it among connections, as long as the default behaviour isn't reconfigured.</param>
		public ApiTokenClient(IAuthConfiguration authConfiguration, HttpClient httpClient = null)
        {
			if (!authConfiguration.RedisConfiguration.Enabled)
			{
				throw new ApplicationException("A cache store needs to be supplied to cache the access token.");
			}

	        _authConfiguration = authConfiguration;
	        _tokenStore = new RedisApiTokenStore(authConfiguration.RedisConfiguration.ConnectionString);
	        _authenticationConnection = new HttpClientAuthenticationConnection(httpClient); 
			_retryPolicy = APIConfiguration.GetAuth0RetryPolicy();
        }
#endif

		public async Task<string> GetAccessToken()
		{
			return await GetAccessToken(_authConfiguration);
		}

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
