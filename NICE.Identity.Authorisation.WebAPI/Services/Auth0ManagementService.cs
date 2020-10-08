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
using Auth0.Core.Collections;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public class Auth0ManagementService : IProviderManagementService
    {
        private readonly ILogger<Auth0ManagementService> _logger;
        private readonly HttpClient _httpClient;

        public Auth0ManagementService(ILogger<Auth0ManagementService> logger, IHttpClientFactory httpClientFactory)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetAccessTokenForManagementAPI()
        {
	        try
	        {
                var client = new AuthenticationApiClient(new Uri($"https://{AppSettings.ManagementAPI.Domain}/"), _httpClient);
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
            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _httpClient);
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
            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _httpClient);

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

        public async Task RevokeRefreshTokensForUser(string nameIdentifier)
        {

	        _logger.LogInformation($"Revoke Refresh Tokens For User {nameIdentifier}");

            var managementApiAccessToken = await GetAccessTokenForManagementAPI();

            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _httpClient);

	        var allDevices = await managementApiClient.DeviceCredentials.GetAllAsync(userId: nameIdentifier, type: "refresh_token");

	        if (allDevices.Any())
	        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		        RevokeRefreshToken(managementApiClient, allDevices.Select(device => device.Id)); //intentionally not awaiting. this might take a while to complete due to rate limiting.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	        }
        }

        private async Task RevokeRefreshToken(ManagementApiClient managementApiClient, IEnumerable<string> refreshTokens)
        {
	        _logger.LogInformation($"Starting Revoking {refreshTokens.Count()} Refresh Tokens");

            foreach (var refreshToken in refreshTokens)
	        {
				await managementApiClient.DeviceCredentials.DeleteAsync(refreshToken); //when we hit the management api too frequently in a short space of time (~ > 10 requests a second), you get a http status 429 error - "too many requests" 
                _logger.LogInformation($"Refresh token: {refreshToken} revoked");

				var rateLimit = managementApiClient.GetLastApiInfo().RateLimit; //this object is set by the headers returned when the DeleteAsync method is hit.
				var utcNow = DateTime.UtcNow;
				var rateLimitResetTimeUtc = rateLimit.Reset.ToUniversalTime();
				var rateLimitCounter = 0;
                if (rateLimit.Remaining <= 1 && rateLimitResetTimeUtc > utcNow)
                {
	                rateLimitCounter++;
                    if (rateLimitCounter > 100) //loop limit here just to be safe, as this is going to execute in the background. this is going to limit revoking a maximum of ~1000 refresh tokens, which seems more than enough.
	                {
		                _logger.LogError($"Rate limiting looped 100 times. this seems way too high. refresh token count: {refreshToken.Length}");
                        break;
	                }
	                var timeSpanToSleep = utcNow - rateLimitResetTimeUtc;
	                if (timeSpanToSleep.TotalMinutes > 10) //typically it'll be a few seconds delay
	                {
		                _logger.LogError($"Rate limiting returned a time in excess of 10 minutes. this seems way too high. refresh token count: {refreshToken.Length}");
                        break;
	                }
	                _logger.LogWarning($"Rate limiting the refresh token revocation. sleep time in seconds: {timeSpanToSleep.TotalSeconds}");
                    Thread.Sleep(timeSpanToSleep);
                }
	        }

            _logger.LogInformation($"Finished Revoking {refreshTokens.Count()} Refresh Tokens");
        }




       

        public async Task<(int totalUsersCount, List<BasicUserInfo> last10Users)> GetLastTenUsersAndTotalCount()
        {
	        _logger.LogInformation($"Getting users from management api");

	        var managementApiAccessToken = await GetAccessTokenForManagementAPI();
	        var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _httpClient);
	        try
	        {
		        var pagination = new PaginationInfo(0, 10, true);


                var pagedUsers = await managementApiClient.Users.GetAllAsync(new GetUsersRequest {Sort = "created_at:-1",}, pagination);

                var last10Users= pagedUsers.Select(user => new BasicUserInfo( nameIdentifier: user.UserId, emailAddress: user.Email)).ToList();
                
		        return (pagedUsers.Paging.Total, last10Users);
	        }
	        catch (Exception e)
	        {
		        _logger.LogError(e.Message);
		        throw new Exception("Error when calling the Management API.", e);
	        }
        }
    }
}