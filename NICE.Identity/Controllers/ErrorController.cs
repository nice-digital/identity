using Microsoft.AspNetCore.Mvc;
using NICE.Identity.ViewModels;
using System.Diagnostics;

namespace NICE.Identity.Controllers
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