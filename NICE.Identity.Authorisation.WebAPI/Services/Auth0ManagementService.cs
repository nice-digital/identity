using System;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public class Auth0ManagementService:IProviderManagementService
    {
        private readonly ILogger<Auth0ManagementService> _logger;
        
        public Auth0ManagementService(ILogger<Auth0ManagementService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateUser(string authenticationProviderUserId, User user)
        {
            _logger.LogInformation($"Update user {authenticationProviderUserId} in auth0");

            try
            {
                var client = new AuthenticationApiClient(new Uri($"https://{AppSettings.ManagementAPI.Domain}/"));
                var managementApiTokenRequest = new ClientCredentialsTokenRequest
                {
                    ClientId = AppSettings.ManagementAPI.ClientId,
                    ClientSecret = AppSettings.ManagementAPI.ClientSecret,
                    Audience = AppSettings.ManagementAPI.ApiIdentifier
                };

                var managementApiToken = await client.GetTokenAsync(managementApiTokenRequest);
                var managementApiClient = new ManagementApiClient(managementApiToken.AccessToken,
                    AppSettings.ManagementAPI.Domain);

                try
                {
                    var userUpdateRequest = new UserUpdateRequest
                    {
                        Blocked = user.IsLockedOut,
                        EmailVerified = user.HasVerifiedEmailAddress,
                        Email = user.EmailAddress,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = $"{user.FirstName} {user.LastName}"
                    };
                    await managementApiClient.Users.UpdateAsync(authenticationProviderUserId, userUpdateRequest);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw new Exception("Error when calling the Management API.", e);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error when calling the Authentication API.", e);
            }
        }
        public async Task DeleteUser(string authenticationProviderUserId)
        {
            _logger.LogInformation($"Delete user {authenticationProviderUserId} from auth0");

            try
            {
                var client = new AuthenticationApiClient(new Uri($"https://{AppSettings.ManagementAPI.Domain}/"));
                var managementApiTokenRequest = new ClientCredentialsTokenRequest
                {
                    ClientId = AppSettings.ManagementAPI.ClientId,
                    ClientSecret = AppSettings.ManagementAPI.ClientSecret,
                    Audience = AppSettings.ManagementAPI.ApiIdentifier
                };

                var managementApiToken = await client.GetTokenAsync(managementApiTokenRequest);
                var managementApiClient = new ManagementApiClient(managementApiToken.AccessToken,
                    AppSettings.ManagementAPI.Domain);

                try
                {
                    await managementApiClient.Users.DeleteAsync(authenticationProviderUserId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw new Exception("Error when calling the Management API.", e);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error when calling the Authentication API.", e);
            }
        }
    }
}