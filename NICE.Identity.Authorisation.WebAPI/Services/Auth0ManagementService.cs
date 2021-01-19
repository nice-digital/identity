using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi.Paging;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public class Auth0ManagementService : IProviderManagementService
    {
        private readonly ILogger<Auth0ManagementService> _logger;
        private readonly HttpClientManagementConnection _managementConnection;
        private readonly HttpClient _httpClient;

        public Auth0ManagementService(ILogger<Auth0ManagementService> logger, IHttpClientFactory httpClientFactory, HttpClientManagementConnection managementConnection)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _httpClient = httpClientFactory.CreateClient();
	        _managementConnection = managementConnection;
        }

        public async Task<string> GetAccessTokenForManagementAPI()
        {
	        try
	        {
		        using (var client = new AuthenticationApiClient(new Uri($"https://{AppSettings.ManagementAPI.Domain}/"),
			        _httpClient))
		        {
			        var managementApiTokenRequest = new ClientCredentialsTokenRequest
			        {
				        ClientId = AppSettings.ManagementAPI.ClientId,
				        ClientSecret = AppSettings.ManagementAPI.ClientSecret,
				        Audience = AppSettings.ManagementAPI.ApiIdentifier
			        };

			        var managementApiToken = await client.GetTokenAsync(managementApiTokenRequest);

			        return managementApiToken.AccessToken;
		        }
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
            try
            {
	            using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _managementConnection))
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
            

            try
            {
	            using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _managementConnection))
	            {
		            await managementApiClient.Users.DeleteAsync(authenticationProviderUserId);
	            }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error when calling the Management API.", e);
            }
        }

        public async Task RevokeRefreshTokensForUser(string nameIdentifier)
        {

	        _logger.LogInformation($"Revoke Refresh Tokens For User {nameIdentifier}");

            var managementApiAccessToken = await GetAccessTokenForManagementAPI();

            using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _managementConnection))
            {
				var allDevices = await managementApiClient.DeviceCredentials.GetAllAsync(userId: nameIdentifier, type: "refresh_token");

	            if (allDevices.Any())
	            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		            RevokeRefreshToken(managementApiClient, allDevices.Select(device => device.Id)); //intentionally not awaiting. this might take a while to complete due to rate limiting.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	            }
            }
        }

        private async Task RevokeRefreshToken(ManagementApiClient managementApiClient, IEnumerable<string> refreshTokens)
        {
	        _logger.LogInformation($"Starting Revoking {refreshTokens.Count()} Refresh Tokens");

            foreach (var refreshToken in refreshTokens)
	        {
		        try
		        {
			        await managementApiClient.DeviceCredentials.DeleteAsync(refreshToken); //when we hit the management api too frequently in a short space of time (~ > 10 requests a second), you get a RateLimitApiException exception (handled below). 
					_logger.LogInformation($"Refresh token: {refreshToken} revoked");
		        }
		        catch (RateLimitApiException rateLimitApiException)
		        {
			        var rateLimit = rateLimitApiException.RateLimit;
			        if (rateLimit.Remaining <= 1 && rateLimit.Reset.HasValue)
			        {
				        var timeSpanToSleep = rateLimit.Reset.Value.Offset;
				        if (timeSpanToSleep.TotalMinutes > 2) //typically it'll be a few seconds delay
				        {
					        _logger.LogError($"Rate limiting returned a time in excess of 2 minutes. this seems way too high. refresh token count: {refreshToken.Length}");
					        break;
				        }
				        _logger.LogWarning($"Rate limiting the refresh token revocation. sleep time in seconds: {timeSpanToSleep.TotalSeconds}");
				        Thread.Sleep(timeSpanToSleep);
			        }

			        try
			        {
				        await managementApiClient.DeviceCredentials.DeleteAsync(refreshToken); 
				        _logger.LogInformation($"Refresh token: {refreshToken} revoked after second try");
			        }
			        catch (Exception exception)
			        {
				        _logger.LogError(exception, $"error deleting refresh token: {refreshToken}, after rate limit exception");
				        break;
					}
		        }
	        }
            _logger.LogInformation($"Finished Revoking {refreshTokens.Count()} Refresh Tokens");
        }

        public async Task<(int totalUsersCount, List<BasicUserInfo> last10Users)> GetLastTenUsersAndTotalCount()
        {
	        _logger.LogInformation($"Getting users from management api");

	        var managementApiAccessToken = await GetAccessTokenForManagementAPI();
	        
	        try
	        {
		        using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _managementConnection))
		        {
			        var pagination = new PaginationInfo(pageNo: 0, perPage: 10, includeTotals: true);

			        const string sortByCreatedDescending = "created_at:-1";
			        var pagedUsers = await managementApiClient.Users.GetAllAsync(new GetUsersRequest {Sort = sortByCreatedDescending,}, pagination);

			        var last10Users = pagedUsers.Select(user => new BasicUserInfo(nameIdentifier: user.UserId, emailAddress: user.Email)).ToList();

			        return (pagedUsers.Paging.Total, last10Users);
		        }
	        }
	        catch (Exception e)
	        {
		        _logger.LogError(e.ToString());
		        throw new Exception("Error when calling the Management API.", e);
	        }
        }
    }
}