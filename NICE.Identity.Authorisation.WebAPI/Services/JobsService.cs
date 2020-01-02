using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using System;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IJobsService
    {
        Task<Job> VerificationEmail(string authenticationProviderUserId);
    }

    public class JobsService : IJobsService
    {

        private readonly ILogger<JobsService> _logger;
        private readonly IProviderManagementService _providerManagementService;

        public JobsService(ILogger<JobsService> logger, IProviderManagementService providerManagementService)
        {
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _providerManagementService = providerManagementService;
        }

        public async Task<Job> VerificationEmail(string authenticationProviderUserId)
        {
			if (string.IsNullOrWhiteSpace(authenticationProviderUserId))
				throw new ArgumentNullException(nameof(authenticationProviderUserId));

            var managementApiAccessToken = await _providerManagementService.GetAccessTokenForManagementAPI();
            var managementApiClient = new ManagementApiClient(managementApiAccessToken, AppSettings.ManagementAPI.Domain);

            try
            {
                var sendVerificationEmail = await managementApiClient.Jobs
                    .SendVerificationEmailAsync(new VerifyEmailJobRequest {
                        UserId = authenticationProviderUserId
                    });

                return sendVerificationEmail;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error when calling the Management API.", e); 
            }
        }
    }
}