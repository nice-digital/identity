#if NETFRAMEWORK
using System.Web.Http.Controllers;
using System.Net.Http;

namespace NICE.Identity.Authentication.Sdk.Attributes
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Http namespace is for use in ApiController's
	///
	/// the only difference between this attribute and the System.Web.Http.AuthorizeAttribute is that this attribute returns
	/// 403's instead of a 401 if the user is logged in and doesn't have access.
	///
	/// The reason this is important for us, is that if a user is logged in but doesn't have the role, then a 401 will result in a challenge
	/// which redirects to the login page, back to the requested page and you get the 401 again and a nasty redirect loop.
	/// returning the proper 403 prevents this loop.
	/// </summary>
	public class AuthoriseApiAttribute : System.Web.Http.AuthorizeAttribute
	{
		public AuthoriseApiAttribute()
		{
		}
		public AuthoriseApiAttribute(string roles)
		{
			this.Roles = roles;
		}

		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
			{
				actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
			}
			else
			{
				base.HandleUnauthorizedRequest(actionContext);
			}
		}
	}
}
#endif