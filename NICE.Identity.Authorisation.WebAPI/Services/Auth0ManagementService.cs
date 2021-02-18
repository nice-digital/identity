using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User = NICE.Identity.Authorisation.WebAPI.DataModels.User;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public class Auth0ManagementService : IProviderManagementService
    {
        private readonly ILogger<Auth0ManagementService> _logger;
        private readonly IManagementConnection _managementConnection;
        private readonly IApiTokenClient _apiTokenClient;
        private readonly AsyncRetryPolicy _retryPolicy;

		public Auth0ManagementService(ILogger<Auth0ManagementService> logger, IManagementConnection managementConnection, IApiTokenClient apiTokenClient)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _managementConnection = managementConnection;
	        _apiTokenClient = apiTokenClient;
	        _retryPolicy = APIConfiguration.GetAuth0RetryPolicy(_logger);
        }
		
		public async Task<string> GetAccessTokenForManagementAPI()
		{
			return await _apiTokenClient.GetAccessToken(AppSettings.ManagementAPI.Domain,
				AppSettings.ManagementAPI.ClientId,
				AppSettings.ManagementAPI.ClientSecret,
				AppSettings.ManagementAPI.ApiIdentifier);
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
		            await _retryPolicy.ExecuteAsync(async () => await managementApiClient.Users.UpdateAsync(authenticationProviderUserId, userUpdateRequest));
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
		            await _retryPolicy.ExecuteAsync(() => managementApiClient.Users.DeleteAsync(authenticationProviderUserId));
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
	        var managementApiAccessToken = await GetAccessTokenForManagementAPI();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			GetAllRefreshTokensAndRevokeThem(nameIdentifier, managementApiAccessToken); //intentionally not awaiting so the logout succeeds, while refresh tokens are still deleted in the background.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		}

		private async Task GetAllRefreshTokensAndRevokeThem(string nameIdentifier, string managementApiAccessToken)
		{
			using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain, _managementConnection))
			{
				var pageNumber = 0;
				const int pageSize = 50;
				IPagedList<DeviceCredential> allDevices;

				var getDeviceCredentialsRequest = new Auth0.ManagementApi.Models.GetDeviceCredentialsRequest()
				{
					UserId = nameIdentifier,
					Type = "refresh_token"
				};

				do
				{
					var pagination = new PaginationInfo(pageNumber, pageSize);

					allDevices = await _retryPolicy.ExecuteAsync(() => managementApiClient.DeviceCredentials.GetAllAsync(getDeviceCredentialsRequest, pagination));

					await RevokeRefreshToken(managementApiClient, allDevices.Select(device => device.Id).ToList());

					pageNumber++;
				} while (allDevices.Count >= pageSize);
			}
		}

        private async Task RevokeRefreshToken(ManagementApiClient managementApiClient, IList<string> refreshTokens)
        {
	        _logger.LogWarning($"Starting Revoking {refreshTokens.Count} Refresh Tokens");

			foreach (var refreshToken in refreshTokens)
	        {
				await _retryPolicy.ExecuteAsync(() => managementApiClient.DeviceCredentials.DeleteAsync(refreshToken));
				_logger.LogWarning($"Refresh token: {refreshToken} revoked");
			}
            _logger.LogWarning($"Finished Revoking {refreshTokens.Count} Refresh Tokens");
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
			      
			        var pagedUsers = await _retryPolicy.ExecuteAsync(() => managementApiClient.Users.GetAllAsync(new GetUsersRequest {Sort = sortByCreatedDescending,}, pagination));

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

		//this function was moved from the verificationemailservice to here, as it belongs here.
		public async Task<Auth0.ManagementApi.Models.Job> VerificationEmail(string authenticationProviderUserId)
		{
			if (string.IsNullOrWhiteSpace(authenticationProviderUserId))
				throw new ArgumentNullException(nameof(authenticationProviderUserId));

			var managementApiAccessToken = await GetAccessTokenForManagementAPI();

			try
			{
				using (var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain))
				{
					var sendVerificationEmail = await _retryPolicy.ExecuteAsync(() => managementApiClient.Jobs.SendVerificationEmailAsync(new VerifyEmailJobRequest{ UserId = authenticationProviderUserId }));

					return sendVerificationEmail;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				throw new Exception("Error when calling the Management API.", e);
			}
		}
	}
}