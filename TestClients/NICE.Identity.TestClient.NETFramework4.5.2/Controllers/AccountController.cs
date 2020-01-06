using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.TestClient.NETFramework452.Controllers
{
	public class AccountController : Controller
	{
		public ActionResult Login(string returnUrl, bool goToRegisterPage = false)
		{
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
				{
                    RedirectUri = returnUrl ?? "/",
					IsPersistent = true,
					Dictionary = { { "register", goToRegisterPage.ToString().ToLower() } }
				},
				AuthenticationConstants.AuthenticationScheme);
			return new HttpUnauthorizedResult();
		}

		[Authorize]
		public void Logout()
		{
			HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			HttpContext.GetOwinContext().Authentication.SignOut(AuthenticationConstants.AuthenticationScheme);
		}

		[Authorize]
		public ActionResult Claims()
		{
			return View();
		}
	}
}