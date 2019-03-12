using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NICE.Identity.TestClient.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase
    {
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "read:messages")]
		[HttpGet]
	    public Publication Get()
		{
			var pub = new Publication
			{
				Id = "1234",
				SomeText = "My publication"
			};

		    return pub;
	    }
    }

	public class Publication
	{
		public string Id { get; set; }

		public string SomeText { get; set; }
	}
}