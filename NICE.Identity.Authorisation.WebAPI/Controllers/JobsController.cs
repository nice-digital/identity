using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Requests;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using Swashbuckle.AspNetCore.Annotations;


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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Send a 'verify email address' email",
            Description = "Send an email to the specified user that asks them to click a link to verify their email address.",
            OperationId = "VerificationEmail"
        )]
        public async Task<ActionResult> VerificationEmail(
            [FromBody]
            [SwaggerParameter(Description = "The userId of the user to whom the email will be sent", Required = true)] 
            VerificationEmail verificationEmail)
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
                var verificationEmailJob = await _jobsService.VerificationEmail(verificationEmail.UserId);

                return Ok(verificationEmailJob);
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