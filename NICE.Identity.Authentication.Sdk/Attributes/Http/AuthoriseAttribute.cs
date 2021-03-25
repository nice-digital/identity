#if NETFRAMEWORK
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Controllers;

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
	///
	/// If the client application has an appSetting matching the one in: Constants.AppSettings.PermissionDeniedRedirectPath
	/// Then this attribute will redirect to that path. The client application will then be in charge of handling the 403 - by splashing up an error page perhaps.
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
				actionContext.Response = GetPermissionDeniedHttpResponseMessage(actionContext.Request);
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

		private static HttpResponseMessage GetPermissionDeniedHttpResponseMessage(HttpRequestMessage httpRequestMessage)
		{
			var permissionDeniedRedirectPath = ConfigurationManager.AppSettings[Constants.AppSettings.PermissionDeniedRedirectPath];

			if (!string.IsNullOrEmpty(permissionDeniedRedirectPath))
			{
				var response = httpRequestMessage.CreateResponse(HttpStatusCode.Redirect);
				response.Headers.Location = new Uri(new Uri(httpRequestMessage.RequestUri.GetLeftPart(System.UriPartial.Authority)), permissionDeniedRedirectPath);
				return response;
			}

			return new HttpResponseMessage(HttpStatusCode.Forbidden);
		}
	}
}
#endif