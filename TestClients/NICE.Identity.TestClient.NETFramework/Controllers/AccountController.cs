using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

namespace NICE.Identity.TestClient.NETFramework.Controllers
{
	public class AccountController : Controller
	{
		public ActionResult Login(string returnUrl)
		{
			var test = Url.Action("Index", "Home");
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
				{
                    RedirectUri = returnUrl ?? test,
					IsPersistent = true
				},
				"Auth0");
			return new HttpUnauthorizedResult();
		}

		[Authorize]
		public void Logout()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			HttpContext.GetOwinContext().Authentication.SignOut("Auth0");
		}

		[Authorize]
		public ActionResult Claims()
		{
			return View();
		}
	}
}