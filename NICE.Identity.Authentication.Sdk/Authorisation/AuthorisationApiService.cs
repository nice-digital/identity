using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.External;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    internal class AuthorisationApiService : IAuthorisationService
    {
        private readonly IHttpClientDecorator _httpClient;
        private readonly Uri _baseUrl;
        
        public AuthorisationApiService(IOptions<AuthorisationServiceConfiguration> configuration, IHttpClientDecorator httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (configuration?.Value == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _baseUrl = new Uri(configuration.Value.BaseUrl);
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
                // TODO: LOG
                throw;
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
                // TODO: LOG
                throw;
            }
        }
    }
}