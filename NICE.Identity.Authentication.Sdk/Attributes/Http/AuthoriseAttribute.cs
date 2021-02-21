#if NETFRAMEWORK
using System.Web.Http.Controllers;
using System.Net.Http;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.Authentication.Sdk.Attributes.Http
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Http namespace is for web controllers or (more usually) ApiController's.
	///
	/// the main difference between this attribute and the System.Web.Http.AuthorizeAttribute is that this attribute returns
	/// 403's instead of a 401 if the user is logged in and doesn't have access.
	///
	/// The reason this is important for us, is that if a user is logged in but doesn't have the role, then a 401 will result in a challenge
	/// which redirects to the login page, back to the requested page and you get the 401 again and a nasty redirect loop.
	/// returning the proper 403 prevents this loop.
	/// </summary>
	public class AuthoriseAttribute : System.Web.Http.AuthorizeAttribute
	{
		public AuthoriseAttribute()
		{
		}
		public AuthoriseAttribute(string roles)
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

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var principal = actionContext.RequestContext.Principal;
			if (principal.Identity.IsAuthenticated && principal.GrantType().Equals(AuthenticationConstants.ClientCredentials))
			{
				//if the granttype is client-credentials, then don't check the roles. so in this case the calling app has used the client_secret, to get an access token,
				//then it's hitting an authorised route for an api call, but the authorize attribute has a role on it too.
				//to simplify things we'll allow it for client_credentials all the time, without having to specify permission scopes for m2m access tokens.
				//the equivalent .net core code is in RoleRequirementHandler.cs
				return;
			}

			base.OnAuthorization(actionContext);
		}
	}
}
#endif