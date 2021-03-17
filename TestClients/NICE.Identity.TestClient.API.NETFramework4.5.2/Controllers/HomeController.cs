using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using System.Web.Mvc;

namespace NICE.Identity.TestClient.API.NETFramework4._5._2.Controllers
{
	public class HomeController : Controller
	{
		private readonly IApiTokenClient _apiTokenClient;

		public HomeController(IApiTokenClient apiTokenClient)
		{
			_apiTokenClient = apiTokenClient;
		}

		public ActionResult Index()
		{
			ViewBag.Title = "Home Page";

			return View();
		}


		public async Task<ActionResult> GetM2MToken()
		{
			ViewBag.Title = "M2M Token";

			ViewBag.M2MToken = await _apiTokenClient.GetAccessToken();

			return View("M2MToken");
		}
	}
}
