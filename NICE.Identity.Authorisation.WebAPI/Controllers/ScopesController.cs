using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ScopesController : ControllerBase
	{
		// GET api/scopes/1234
		[HttpGet("{userId}")]
		public async Task<ActionResult<Models.Responses.Scope[]>> Get(int userId)
		{
            throw new NotImplementedException();

		    return Ok();
		}

	    // GET api/scopes/1234
	    [HttpPut("{userId}")]
	    public async Task<ActionResult<Models.Responses.Scope[]>> CreateOrUpdate(int userId, [FromBody]Models.Requests.Scope scope)
	    {
	        throw new NotImplementedException();

	        return Ok();
	    }
    }
}