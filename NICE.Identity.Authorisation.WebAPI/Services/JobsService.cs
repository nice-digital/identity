using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IJobsService
    {
        Task<Job> VerificationEmail(string authenticationProviderUserId);
    }

    public class JobsService : IJobsService
    {
        private readonly IManagementApiService _managementApiService = new ManagementApiService();

        public JobsService()
        {

        }

        public async Task<Job> VerificationEmail(string authenticationProviderUserId)
        {
            try
            {
                var managementApiToken = await _managementApiService.GetToken();
                var managementApiClient = new ManagementApiClient(managementApiToken.AccessToken, AppSettings.ManagementAPI.Domain);

                try
                {
                    var sendVerificationEmail = managementApiClient.Jobs.SendVerificationEmailAsync(new VerifyEmailJobRequest
                    {
                        UserId = authenticationProviderUserId
                    });
                    return sendVerificationEmail.Result;
                }
                catch (Exception e)
                {
                    throw new Exception("Error when calling the Management API; see inner exception.", e);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error when getting the Management API Token; see inner exception.", e);
            }
        }
    }
}
