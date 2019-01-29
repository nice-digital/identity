using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Areas.Admin.ViewModels;

namespace NICE.Identity.Areas.Admin.Controllers
{
	public class HomeController : AdministrationControllerBase
	{
		//GET / (home is set as the default controller in Startup MapRoute
		public IActionResult Index()
        {
	        var model = new HomeViewModel("IdAM Admin", "Admin", true);
            return View("Index", model);
        }
    }
}