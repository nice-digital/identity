using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NICE.Identity.NETFramework.Nuget;

namespace NICE.Identity.TestClient.NETFramework.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[AuthoriseRole(Roles = "Editor")]
		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		//[AuthoriseRole(Roles = "Administrator")]
		[AuthoriseRole(Roles = "Administrator")]
		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}