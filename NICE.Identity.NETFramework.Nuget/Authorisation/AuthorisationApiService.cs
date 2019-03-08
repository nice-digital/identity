using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NICE.Identity.NETFramework.Nuget.Abstractions;
using NICE.Identity.NETFramework.Nuget.Domain;
using NICE.Identity.NETFramework.Nuget.External;

namespace NICE.Identity.NETFramework.Nuget.Authorisation
{

	public class AuthorisationApiService : IAuthorisationService
    {
        private const string RoleClaimTypeName = "Role";

        private readonly IHttpClientDecorator _httpClient;
        private readonly Uri _baseUrl;

        public AuthorisationApiService(IOptions<AuthorisationServiceConfiguration> configuration,
            IHttpClientDecorator httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (configuration?.Value == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _baseUrl = new Uri(configuration.Value.BaseUrl);
        }

	    public AuthorisationApiService(string baseUrl)
	    {
		    _baseUrl = new Uri(baseUrl);
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

        public async Task<bool> UserSatisfiesAtLeastOneRoleAsync(string userId, IEnumerable<string> roles)
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
	        return UserHasRole(roles, claims);
		}
	    public bool UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roles)
	    {
		    IEnumerable<Claim> claims;

		    var uri = new Uri(_baseUrl, string.Format(Constants.AuthorisationURLs.GetClaims, userId));

		    try
		    {

				var webClient = new WebClient();
			    var response = webClient.DownloadString(uri);
			    claims = JsonConvert.DeserializeObject<Claim[]>(response);
		    }
		    catch (Exception e)
		    {
			    throw new Exception("Error when calling Authorisation service; see inner exception.", e);
		    }
		    return UserHasRole(roles, claims);
	    }

	    private static bool UserHasRole(IEnumerable<string> roles, IEnumerable<Claim> claims)
	    {
		    var roleFound = false;

		    foreach (var role in roles)
		    {
			    roleFound = claims.Any(x => x.Type == RoleClaimTypeName &&
			                                  x.Value == role);

			    if (roleFound)
			    {
				    break;
			    }
		    }
		    return roleFound;
		}
    }
}