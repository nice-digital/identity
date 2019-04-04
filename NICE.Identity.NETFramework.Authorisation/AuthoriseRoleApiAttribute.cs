using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace NICE.Identity.NETFramework.Authorisation
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Http namespace is for use in ApiController's
	/// 
	/// </summary>
	public class AuthoriseApiAttribute : System.Web.Http.AuthorizeAttribute
	{
		private readonly IAuthorisationService authService;

		public AuthoriseApiAttribute(string roles)
		{
			Roles = roles;
			authService = DependencyResolver.Current.GetService<IAuthorisationService>();
        }

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var principal = actionContext.RequestContext.Principal;
			if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated && principal.Identity is ClaimsIdentity)
			{
				var result = NiceAuthorisationClaims.ProcessClaims(principal, authService, Roles.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));

                actionContext.Request.Properties["UserId"] = result.IdClaim.Value;
                actionContext.Request.Properties["Name"] = result.Name;

                if (result.Result.Equals(NiceAuthorisationClaims.ClaimResult.Successful))
	                return; //successfully verified user has role.

				if (result.Result.Equals(NiceAuthorisationClaims.ClaimResult.Forbidden))
					actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Permission denied"); //403

				return;
			}

			HandleUnauthorizedRequest(actionContext); //401 - which will likely result in a 302 redirect to the login page.
		}
	}
}