using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ClaimsController : ControllerBase
	{
		private readonly IClaimsService _claimsService;
		private readonly ILogger<ClaimsController> _logger;

		public ClaimsController(IClaimsService claimsService, ILogger<ClaimsController> logger)
	    {
		    _claimsService = claimsService ?? throw new ArgumentNullException(nameof(claimsService));
		    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		// GET api/claims/someuserid
	    [HttpGet("{authenticationProviderUserId}")]
	    public async Task<ActionResult<IEnumerable<ApiModels.Responses.Claim[]>>> Get(string authenticationProviderUserId)
	    {
	        try
	        {
	            var result = _claimsService.GetClaims(authenticationProviderUserId);

	            if (result == null)
	            {
	                return StatusCode(404, "User not found");
	            }

	            return Ok(result);
	        }
	        catch (Exception e)
	        {
	            return StatusCode(503, e.Message);
	        }
	    }

	    [HttpPost("import/{websiteHost}/{roleName}")]
		[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Consumes("application/json")]
		[Produces("application/json")]
		public IActionResult ImportUserRoles([FromBody] IEnumerable<ImportUser> usersToImport, string websiteHost, string roleName)
		{
			if (!ModelState.IsValid)
			{
				var serializableModelState = new SerializableError(ModelState);
				var modelStateJson = JsonConvert.SerializeObject(serializableModelState);
				_logger.LogError($"Invalid model for create user", modelStateJson);
				return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid model for create user" });
			}
			if (string.IsNullOrEmpty(websiteHost))
				return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid website host supplied" });
			if (string.IsNullOrEmpty(roleName))
				return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid role name supplied" });

			try
			{
				_claimsService.ImportUserRoles(usersToImport, websiteHost, roleName);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
			}
		}
	}
}