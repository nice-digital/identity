using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authorisation.WebAPI.APIModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StatusController : ControllerBase
	{
		private readonly IClaimsService _claimsService;

		public StatusController(IClaimsService claimsService)
	    {
		    _claimsService = claimsService ?? throw new ArgumentNullException(nameof(claimsService));
	    }

		// GET api/status/get
		[AllowAnonymous]
		[HttpGet]
		[Produces("application/json")]
		public ActionResult<Status> Get()
	    {
	        try
	        {
		        if (!this.User.Identity.IsAuthenticated)
		        {
			        return new ActionResult<Status>(new Status(isAuthenticated: false, displayName: null,
				        links: new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Sign in", "todo: sign in link") }));
				}

				//todo: get the other links.

				return new ActionResult<Status>(new Status(isAuthenticated: true, displayName: this.User.Identity.Name,
			        links: new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Sign out", "todo: sign out link") }));
				
	        }
	        catch (Exception e)
	        {
	            return StatusCode(503, e.Message);
	        }
	    }
    }
}