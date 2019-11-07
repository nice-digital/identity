using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class AuthorisationApiService : IAuthorisationService
    {
        private const string RoleClaimTypeName = "Role";

        private readonly IHttpClientDecorator _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthConfiguration _authConfiguration;
        private readonly Uri _baseUrl;
        private const int DefaultTokenExpirationInSeconds = 86400; //86400 is 24 hours in seconds

        public AuthorisationApiService(IAuthConfiguration authConfiguration, IHttpClientDecorator httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor;
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

		/// <summary>
		/// The "host" is the domain name eg. "www.nice.org.uk" of the website you're trying to check the role against.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roles"></param>
		/// <param name="host"></param>
		/// <returns></returns>
        public async Task<bool> UserSatisfiesAtLeastOneRoleForAGivenHost(string userId, IEnumerable<string> roles, string host)
        {
            IEnumerable<Claim> claims;
            bool userHasRole = false;
			var uri = new Uri(_baseUrl, string.Format(Constants.AuthorisationURLs.GetClaims, userId));

			var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
			_httpClient.AddBearerToken(new JwtToken { AccessToken = token, ExpiresIn = DefaultTokenExpirationInSeconds, TokenType = _authConfiguration.GrantTypeForMachineToMachine });

			try
			{
                var responseMessage = await _httpClient.GetAsync(uri);

                if (responseMessage.IsSuccessStatusCode)
                {
	                claims = JsonConvert.DeserializeObject<Claim[]>(await responseMessage.Content.ReadAsStringAsync());
				}
                else if (responseMessage.StatusCode == HttpStatusCode.NotFound) //user not found.
                {
	                return false;
                }
                else
                {
	                throw new Exception($"Error calling authorisation service. Response status: {responseMessage.StatusCode}");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error when calling Authorisation service; see inner exception.", e);
            }

            foreach (var role in roles)
            {
                userHasRole = claims.Any(x => x.Type == RoleClaimTypeName &&
                                              string.Equals(x.Issuer, host, StringComparison.OrdinalIgnoreCase) &&
                                              x.Value == role
                );

                if (userHasRole)
                {
                    break;
                }
            }
            return userHasRole;
        }
    }
}