using NICE.Identity.NETFramework.Nuget;
using System.Web.Mvc;

namespace NICE.Identity.TestClient.NETFramework.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[AuthoriseRole("Editor")]
		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		[AuthoriseRole("")]
		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}