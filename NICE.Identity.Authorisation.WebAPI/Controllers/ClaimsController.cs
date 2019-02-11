using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authorisation.WebAPI.Abstractions;
using NICE.Identity.Authorisation.WebAPI.Domain;
using NICE.Identity.Authorisation.WebAPI.Models.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using Claim = NICE.Identity.Authorisation.WebAPI.Models.Requests.Claim;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClaimsController : ControllerBase
	{
	    private readonly IRoleRepository _roleRepository;
		private readonly IClaimsService _claimsService;

		public ClaimsController(IRoleRepository roleRepository, IClaimsService claimsService)
	    {
	        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
		    _claimsService = claimsService;
	    }

		// GET api/claims/1234
		[HttpGet("{userId}")]
		public async Task<ActionResult<IEnumerable<Models.Responses.Claim[]>>> Get(int userId)
		{
			var result = _claimsService.GetClaims(userId);
		    return Ok(result);
		}

	    // PUT api/claims
	    [HttpPut("{userId}")]
	    public async Task<ActionResult> CreateOrUpdate(int userId, [FromBody] Models.Requests.Claim claim)
	    {
	        var role = MapClaimToRole(claim);

	        try
	        {
	            await _roleRepository.AddToUser(role);
            }
	        catch (Exception e)
	        {
	            // TODO: Implement Logging and custom exception types

	            var error = new ErrorDetail()
	            {
                    ErrorMessage = e.Message
	            };

	            return StatusCode(503, error);
	        }

	        return Ok();
	    }

	    private Role MapClaimToRole(Claim claim)
	    {
	        var role = new Role()
	        {
	            Id = claim.RoleId,
	            Name = claim.RoleName
	        };

	        return role;
	    }
    }
}