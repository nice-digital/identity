using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public class Auth0ManagementService : IProviderManagementService
    {
        private readonly ILogger<Auth0ManagementService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public Auth0ManagementService(ILogger<Auth0ManagementService> logger, IHttpClientFactory httpClientFactory)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAccessTokenForManagementAPI()
        {
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

		        return managementApiToken.AccessToken;
	        }
	        catch (Exception e)
	        {
		        _logger.LogError(e.Message);
		        throw new Exception("Error in GetAccessTokenForManagementAPI when calling the Management API.", e);
	        }
        }

        public async Task UpdateUser(string authenticationProviderUserId, User user)
        {
            _logger.LogInformation($"Update user {authenticationProviderUserId} in auth0");

            var managementApiAccessToken = await GetAccessTokenForManagementAPI();
            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain);
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

        public async Task DeleteUser(string authenticationProviderUserId)
        {
            _logger.LogInformation($"Delete user {authenticationProviderUserId} from auth0");

            var managementApiAccessToken = await GetAccessTokenForManagementAPI();
            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain);

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

        private async Task RevokeRefreshToken(string refreshToken)
        {
	        var client = _httpClientFactory.CreateClient();
            var uri = new Uri($"https://{AppSettings.ManagementAPI.Domain}/oauth/revoke");

	        var postObject = new
	        {
		        client_id = AppSettings.ManagementAPI.ClientId, //TODO: this is the wrong application. 
		        client_secret = AppSettings.ManagementAPI.ClientSecret, //and secret.
		        token = refreshToken
	        };

	        var request = new HttpRequestMessage(HttpMethod.Post, uri)
	        {
		        Content = new StringContent(JsonConvert.SerializeObject(postObject), Encoding.UTF8, "application/json")
	        };
	        var responseMessage = await client.SendAsync(request);
	        if (!responseMessage.IsSuccessStatusCode)
	        {
                throw new Exception($"Error {(int) responseMessage.StatusCode} trying to revoke refresh token: {refreshToken}");
	        }
        }

        public async Task RevokeRefreshTokensForUser(string nameIdentifier)
        {
	        var managementApiAccessToken = await GetAccessTokenForManagementAPI();
	        var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain);

            //annoyingly we can't use the device credential api, as it's rate limited, but doesn't return the X-RateLimit-Remaining header so, is basically unusable.
	        var allDevices = await managementApiClient.DeviceCredentials.GetAllAsync(userId: nameIdentifier);

	        var firstRefreshToken = allDevices.FirstOrDefault(device => device.Type.Equals("refresh_token"))?.Id;

	        if (!string.IsNullOrEmpty(firstRefreshToken))
	        {
		        await RevokeRefreshToken(firstRefreshToken);
	        }
        }
    }
}