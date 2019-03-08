using NICE.Identity.NETFramework.Nuget;
using System.Web.Http;

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
		[AuthoriseApi("Administrator")]
	    public IHttpActionResult SecuredAdministrator()
	    {

		    return Ok(new { secured = true });
	    }

		// GET /api/secured-editor
		[HttpGet]
	    [Route("secured-editor")]
	    [AuthoriseApi("Editor")]
	    public IHttpActionResult SecuredEditor()
	    {

		    return Ok(new { secured = true });
	    }
	}
}
