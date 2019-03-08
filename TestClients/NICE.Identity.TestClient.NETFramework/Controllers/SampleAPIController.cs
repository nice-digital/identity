using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NICE.Identity.NETFramework.Nuget;

namespace NICE.Identity.TestClient.NETFramework.Controllers
{
	[RoutePrefix("api")]
	public class SampleApiController : ApiController
    {

		// GET /api/unsecured
		[HttpGet]
	    [Route("unsecured")]
	    public IHttpActionResult Unsecured()
	    {
		    
		    return Ok(new { Unsecured = true});
	    }

		// GET /api/secured
		[HttpGet]
		[Route("secured")]
		[AuthoriseRoleApi("Administrator")]
	    public IHttpActionResult SecuredAdministrator()
	    {

		    return Ok(new { secured = true });
	    }

		// GET /api/secured-editor
		[HttpGet]
	    [Route("secured-editor")]
	    [AuthoriseRoleApi("Editor")]
	    public IHttpActionResult SecuredEditor()
	    {

		    return Ok(new { secured = true });
	    }
	}
}
