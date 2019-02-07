using System;
using Microsoft.AspNetCore.Mvc;
using z_deprecated_NICE.Identity.ViewModels;

namespace z_deprecated_NICE.Identity.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View(new HomeViewModel("Home", "NICE Identity", false));
		}

		/// <summary>
		/// GET /Home/RaiseError
		/// 
		/// this is just here to test the ELK stack. 
		/// </summary>
		/// <returns></returns>
		public IActionResult RaiseError()
		{
			throw new Exception("Exception raised by Home/RaiseError");
		}
	}
}
