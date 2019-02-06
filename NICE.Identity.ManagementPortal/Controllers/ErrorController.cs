using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using z_deprecated_NICE.Identity.ViewModels;

namespace z_deprecated_NICE.Identity.Controllers
{
	public class ErrorController : Controller
    {
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Index()
		{
			return View(new ErrorViewModel("Error", "Error", requestId: Activity.Current?.Id ?? HttpContext.TraceIdentifier));
		}
	}
}