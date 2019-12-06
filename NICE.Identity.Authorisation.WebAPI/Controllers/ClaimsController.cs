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
	    public async Task<ActionResult<IEnumerable<ApiModels.Claim[]>>> Get(string authenticationProviderUserId)
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
	}
}