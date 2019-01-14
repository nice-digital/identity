using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Areas.Admin.Models;

namespace NICE.Identity.Areas.Admin.Controllers
{
    public class HomeController : AdministrationControllerBase
	{
        public IActionResult Index()
        {
	        var model = new HomeViewModel("IdAM Admin", "Admin", true);
            return View("Index", model);
        }
    }
}