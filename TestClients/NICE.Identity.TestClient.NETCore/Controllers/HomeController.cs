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

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		[Authorize(Policy = PolicyTypes.Editor)]
		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		//[Authorize(Roles = "Administrator,EditorSpecial")]
		//[Authorize(Policy = PolicyTypes.Administrator)]
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}