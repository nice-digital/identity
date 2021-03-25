#if NETFRAMEWORK
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NICE.Identity.Authentication.Sdk.Attributes.Mvc
{
	/// <summary>
	/// This authorise attribute subclass is used by regular .net framework controllers (i.e. not api controllers).
	/// 
	/// the only difference between this attribute and the System.Web.Mvc.AuthorizeAttribute is that this attribute returns
	/// 403's instead of a 401 if the user is logged in and doesn't have access.
	///
	/// The reason this is important for us, is that if a user is logged in but doesn't have the role, then a 401 will result in a challenge
	/// which redirects to the login page, back to the requested page and you get the 401 again and a nasty redirect loop.
	/// returning the proper 403 prevents this loop.
	///
	/// If the client application has an appSetting matching the one in: Constants.AppSettings.PermissionDeniedRedirectPath
	/// Then this attribute will redirect to that path. The client application will then be in charge of handling the 403 - by splashing up an error page perhaps.
	/// </summary>
	public class AuthoriseAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		public AuthoriseAttribute() { }
		public AuthoriseAttribute(string roles)
		{
			this.Roles = roles;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				filterContext.Result = GetPermissionDeniedActionResult(filterContext.RequestContext.HttpContext.Request);
			}
			else
			{
				base.HandleUnauthorizedRequest(filterContext);
			}
		}

		private static ActionResult GetPermissionDeniedActionResult(HttpRequestBase httpRequest)
		{
			var permissionDeniedRedirectPath = ConfigurationManager.AppSettings[Constants.AppSettings.PermissionDeniedRedirectPath];

			if (!string.IsNullOrEmpty(permissionDeniedRedirectPath))
			{
				return new RedirectResult(permissionDeniedRedirectPath);
			}

			return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
		}
	}
}
#endif