using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.External;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    internal class AuthorisationApiService : IAuthorisationService
    {
        private readonly IHttpClientDecorator _httpClient;
        private readonly string _baseUrl;

        public AuthorisationApiService(IOptions<AuthorisationServiceConfiguration> configuration, IHttpClientDecorator httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (configuration?.Value == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _baseUrl = configuration.Value.BaseUrl;
        }

        public async Task<IEnumerable<Role>> GetByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task AddToUser(Role role)
        {
            throw new NotImplementedException();
        }
    }
}