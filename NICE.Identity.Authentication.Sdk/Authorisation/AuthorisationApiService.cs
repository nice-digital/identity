using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.External;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class AuthorisationApiService : IAuthorisationService
    {
        private const string RoleClaimTypeName = "Role";

        private readonly IHttpClientDecorator _httpClient;
        private readonly IAuthConfiguration _authConfiguration;
        private readonly Uri _baseUrl;

        public AuthorisationApiService(IAuthConfiguration authConfiguration,
            IHttpClientDecorator httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authConfiguration = authConfiguration;

            if (_authConfiguration?.WebSettings.AuthorisationServiceUri == null)
            {
                throw new ArgumentNullException(nameof(_authConfiguration));
            }

            _baseUrl = new Uri(_authConfiguration.WebSettings.AuthorisationServiceUri);
        }

        public async Task<IEnumerable<Claim>> GetByUserId(string userId)
        {
            IEnumerable<Claim> claims;

            var uri = new Uri(_baseUrl, userId);

            try
            {
                var response = await _httpClient.GetStringAsync(uri);
                claims = JsonConvert.DeserializeObject<Claim[]>(response);
            }
            catch (Exception e)
            {
                throw new Exception("Error when calling Authorisation service; see inner exception.", e);
            }

            return claims;
        }

        public async Task CreateOrUpdate(string userId, Claim role)
        {
            var uri = new Uri(_baseUrl, userId);

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

            var uri = new Uri(_baseUrl, string.Format(Constants.AuthorisationURLs.GetClaims, userId));

            try
            {

				var response = await _httpClient.GetStringAsync(uri);
                claims = JsonConvert.DeserializeObject<Claim[]>(response);
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