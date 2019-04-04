using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.External;
using NICE.Identity.NETFramework.Authorisation;


namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class AuthorisationApiService : IAuthorisationService
    {
        private const string RoleClaimTypeName = "Role";

        private readonly IHttpClientDecorator _httpClient;

        public AuthorisationApiService(IHttpClientDecorator httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public AuthorisationApiService(IHttpClientFactory client)
        {
            var _client = client.CreateClient("Auth0ServiceApiClient");
            _httpClient = new HttpClientDecorator(_client);
        }

        public async Task<IEnumerable<Claim>> GetByUserId(string userId)
        {
            IEnumerable<Claim> claims;
            
            var uri = new Uri(_httpClient.BaseUrl, userId);

            try
            {
                var response = await _httpClient.GetStringAsync(uri);
                claims = JsonConvert.DeserializeObject<Claim[]>(response, new ClaimConverter());
            }
            catch (Exception e)
            {
                throw new Exception("Error when calling Authorisation service; see inner exception.", e);
            }

            return claims;
        }

        public async Task CreateOrUpdate(string userId, Claim role)
        {
            var uri = new Uri(_httpClient.BaseUrl, userId);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(role), Encoding.UTF8, "application/json");

                await _httpClient.PutAsync(uri, content);
            }
            catch (Exception e)
            {
                throw new Exception("Error when calling Authorisation service; see inner exception.", e);
            }
        }

        public async Task<bool> UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roles)
        {
            IEnumerable<Claim> claims;

            var uri = new Uri(_httpClient.BaseUrl, string.Format(Constants.AuthorisationURLs.GetClaims, userId));

            try
            {

				var response = await _httpClient.GetStringAsync(uri);
                claims = JsonConvert.DeserializeObject<Claim[]>(response, new ClaimConverter());
            }
            catch (Exception e)
            {
                throw new Exception("Error when calling Authorisation service; see inner exception.", e);
            }

            bool userHasRole = false;

            foreach (var role in roles)
            {
                userHasRole = claims.Any(x => x.Type == RoleClaimTypeName &&
                                              x.Value == role);

                if (userHasRole)
                {
                    break;
                }
            }
            
            return userHasRole;
        }
    }
}