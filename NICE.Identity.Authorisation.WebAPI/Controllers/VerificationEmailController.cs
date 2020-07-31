using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using NICE.Identity.Authorisation.WebAPI.Services;
using Swashbuckle.AspNetCore.Annotations;


namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
	public class VerificationEmailController : ControllerBase
    {
        private readonly ILogger<VerificationEmailController> _logger;
        private readonly IVerificationEmailService _verificationEmailService;

        public VerificationEmailController(IVerificationEmailService verificationEmailService, ILogger<VerificationEmailController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _verificationEmailService = verificationEmailService ?? throw new ArgumentNullException(nameof(verificationEmailService));
        }

        [AllowAnonymous]
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
                _logger.LogError($"Invalid Model State at VerificationEmailController VerificationEmail for UserId: {verificationEmail.UserId}", modelStateJson);

                return BadRequest("Request failed validation");
            }

            try
            {
                var verificationEmailJob = await _verificationEmailService.VerificationEmail(verificationEmail.UserId);

                return Ok(verificationEmailJob);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails()
                {
                    Status = 500, 
                    Title = e.Message, 
                    Detail = e.InnerException?.Message
                });
            }
        }
    }
}