using System;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.Controllers;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IJobsService
    {
        Task<Job> VerificationEmail(string authenticationProviderUserId);
    }

    public class JobsService : IJobsService
    {

        private readonly ILogger<JobsService> _logger;
        
        public JobsService(ILogger<JobsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Job> VerificationEmail(string authenticationProviderUserId)
        {
            var client = new AuthenticationApiClient(new Uri($"https://{AppSettings.ManagementAPI.Domain}/"));
            var managementApiTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = AppSettings.ManagementAPI.ClientId,
                ClientSecret = AppSettings.ManagementAPI.ClientSecret,
                Audience = AppSettings.ManagementAPI.ApiIdentifier
            };
            try
            {
                var managementApiToken = await client.GetTokenAsync(managementApiTokenRequest);
                var managementApiClient = new ManagementApiClient(managementApiToken.AccessToken,
                    AppSettings.ManagementAPI.Domain);

                try
                {
                    var sendVerificationEmail = await managementApiClient.Jobs
                        .SendVerificationEmailAsync(new VerifyEmailJobRequest {UserId = authenticationProviderUserId});

                    return sendVerificationEmail;
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