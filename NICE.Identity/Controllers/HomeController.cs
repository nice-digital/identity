using Microsoft.AspNetCore.Mvc;
using NICE.Identity.ViewModels;
using System.Diagnostics;

namespace NICE.Identity.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View(new HomeViewModel("Home", "NICE Identity", false));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel("Error", "Error", requestId: Activity.Current?.Id ?? HttpContext.TraceIdentifier));
		}
	}
}
