using System.Web.Http;
using NICE.Identity.Authentication.Sdk.Attributes;

namespace NICE.Identity.TestClient.NETFramework461.Controllers
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
		[AuthoriseApi(Roles = "Administrator")]
		public IHttpActionResult SecuredAdministrator()
		{

			return Ok(new { secured = true });
		}

		// GET /api/secured-editor
		[HttpGet]
		[Route("secured-editor")]
		[AuthoriseApi(Roles = "Editor")]
		public IHttpActionResult SecuredEditor()
		{

			return Ok(new { secured = true });
		}
	}
}