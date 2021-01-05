using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.TokenStore;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class ApiTokenClient
    {
        private readonly IAuthConfiguration _authConfiguration;
        private readonly IApiTokenStore _tokenStore;
        private string TokenStoreKey { get; set; }

        #if NETSTANDARD2_0 || NETCOREAPP3_1
        private readonly IHttpClientFactory _clientFactory;

        public ApiTokenClient(IApiTokenStore tokenStore, IHttpClientFactory clientFactory, IAuthConfiguration authConfiguration)
        {
            _tokenStore = tokenStore;
            _clientFactory = clientFactory;
            _authConfiguration = authConfiguration;
        }
        #else
        public ApiTokenClient(IAuthConfiguration authConfiguration)
        {
            _tokenStore = new RedisApiTokenStore(authConfiguration.RedisConfiguration.ConnectionString);
            _authConfiguration = authConfiguration;
        }
        #endif

        public async Task<string> GetAccessToken()
        {
            var token = await _tokenStore.RetrieveAsync(TokenStoreKey);

            if (token != null)
            {
                return token.AccessToken;
            }
            else
            {
                var tokenFromIdentityProvider = await GetTokenFromIdentityProvider(_authConfiguration);
                TokenStoreKey = await _tokenStore.StoreAsync(tokenFromIdentityProvider);
                return tokenFromIdentityProvider.AccessToken;
            }
        }

        private async Task<JwtToken> GetTokenFromIdentityProvider(IAuthConfiguration authConfiguration)
        {
            var tokenRequestFormContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", $"{authConfiguration.WebSettings.ClientId}"},
                {"client_secret", $"{authConfiguration.WebSettings.ClientSecret}"},
                {"audience", $"{authConfiguration.MachineToMachineSettings.ApiIdentifier}"},
                {"grant_type", "client_credentials"}
            });
            tokenRequestFormContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, 
                new Uri($"https://{authConfiguration.TenantDomain}/oauth/token"))
            {
                Content = tokenRequestFormContent
            };

            #if NETSTANDARD2_0 || NETCOREAPP3_1
            var tokenClient = _clientFactory.CreateClient();
            #else
            var tokenClient = new HttpClient();
            #endif
            
            var tokenResponse = await tokenClient.SendAsync(request);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JwtToken>(tokenJson);
            }
            else
            {
                throw new Exception($"{tokenResponse.StatusCode}:{tokenResponse.ReasonPhrase}");
            }
        }
    }
}
