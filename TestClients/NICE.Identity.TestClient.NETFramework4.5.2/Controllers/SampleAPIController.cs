using System.Web.Http;

namespace NICE.Identity.TestClient.NETFramework452.Controllers
{
	[RoutePrefix("api")]
	public class SampleApiController : ApiController
	{

		// GET /api/unsecured
		[HttpGet]
		[Route("unsecured")]
		public IHttpActionResult Unsecured()
		{

			return Ok(new { Unsecured = true });
		}

		// GET /api/secured
		[HttpGet]
		[Route("secured")]
		[Authorize(Roles = "Administrator")]
		public IHttpActionResult SecuredAdministrator()
		{

			return Ok(new { secured = true });
		}

		// GET /api/secured-editor
		[HttpGet]
		[Route("secured-editor")]
		[Authorize(Roles = "Editor")]
		public IHttpActionResult SecuredEditor()
		{

			return Ok(new { secured = true });
		}
	}
}