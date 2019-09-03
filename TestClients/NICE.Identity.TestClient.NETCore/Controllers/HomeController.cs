using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.TestClient.NetCore.Models;

namespace NICE.Identity.TestClient.NetCore.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult UserProfile()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[ApiExplorerSettings(IgnoreApi = true)]
        [Route("/signin-auth0")]
		public IActionResult CallBack()
		{
			return RedirectToAction("Index");
		}
    }
}