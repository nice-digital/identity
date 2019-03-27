using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobsService _jobsService;

        public JobsController(IJobsService jobsService, ILogger<JobsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobsService = jobsService ?? throw new ArgumentNullException(nameof(jobsService));
        }

        // POST: api/Jobs/VerificationEmail
        // [Authorize]
        /// <summary>
        /// Send a "verify email address" email
        /// </summary>
        /// <param name="body">The user_id of the user to whom the email will be sent</param>
        [HttpPost]
        public async Task<ActionResult> VerificationEmail([FromBody] VerificationEmail verificationEmail)
        {
            if (!ModelState.IsValid)
            {
                var serializableModelState = new SerializableError(ModelState);
                var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
                _logger.LogError($"Invalid Model State at JobsController VerificationEmail for UserId: {verificationEmail.UserId}", modelStateJson);

                return BadRequest("Request failed validation");
            }

            try
            {
                await _jobsService.VerificationEmail(verificationEmail.UserId);

                return Ok();
            }
            catch (Exception e)
            {
                var error = new ErrorDetail()
                {
                    ErrorMessage = e.Message
                };

                return StatusCode(500, error);
            }
        }
    }
}
