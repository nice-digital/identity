using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ClaimsController : ControllerBase
	{
		private readonly IClaimsService _claimsService;
        private readonly IUsersService _usersService;
        private readonly ILogger<ClaimsController> _logger;

		public ClaimsController(IClaimsService claimsService, IUsersService usersService, ILogger<ClaimsController> logger)
	    {
		    _claimsService = claimsService ?? throw new ArgumentNullException(nameof(claimsService));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		// GET api/claims/someuserid
	    [HttpGet("{authenticationProviderUserId}")]
	    public async Task<ActionResult<IEnumerable<ApiModels.Claim[]>>> Get(string authenticationProviderUserId)
	    {
	        try
            {

                await _usersService.UpdateFieldsDueToLogin(HttpUtility.UrlDecode(authenticationProviderUserId));

                var result = _claimsService.GetClaims(HttpUtility.UrlDecode(authenticationProviderUserId));

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