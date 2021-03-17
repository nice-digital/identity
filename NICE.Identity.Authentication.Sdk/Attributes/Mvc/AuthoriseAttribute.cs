#if NETFRAMEWORK
using System.Net;
using System.Web.Mvc;

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
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			}
			else
			{
				base.HandleUnauthorizedRequest(filterContext);
			}
		}
	}
}
#endif