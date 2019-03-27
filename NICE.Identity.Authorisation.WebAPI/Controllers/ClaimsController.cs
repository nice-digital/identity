using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authorisation.WebAPI.Services;
using Claim = NICE.Identity.Authorisation.WebAPI.ApiModels.Requests.Claim;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClaimsController : ControllerBase
	{
		private readonly IClaimsService _claimsService;

		public ClaimsController(IClaimsService claimsService)
	    {
		    _claimsService = claimsService ?? throw new ArgumentNullException(nameof(claimsService));
	    }

		// GET api/claims/someuserid
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        // PUT api/claims
        [HttpPut("{userId}")]
	    public async Task<ActionResult> Put(int userId, [FromBody] Claim claim)
	    {
	        //var role = MapClaimToRole(claim);

	        //try
	        //{
	        //    await _claimsService.AddToUser(role);
         //   }
	        //catch (Exception e)
	        //{
	        //    // TODO: Implement Logging and custom exception types

	        //    var error = new ErrorDetail()
	        //    {
         //           ErrorMessage = e.Message
	        //    };

	        //    return StatusCode(503, error);
	        //}

	        return Ok();
	    }

	    //private Role MapClaimToRole(Claim claim)
	    //{
	    //    var role = new Role()
	    //    {
	    //        Id = claim.RoleId,
	    //        Name = claim.RoleName
	    //    };

	    //    return role;
	    //}
    }
}