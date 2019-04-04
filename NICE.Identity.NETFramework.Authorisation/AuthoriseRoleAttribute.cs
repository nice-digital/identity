using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Web.Mvc;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace NICE.Identity.NETFramework.Authorisation
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Mvc namespace is for use in Web Controllers 
	/// </summary>
	public class AuthoriseAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		private readonly IAuthorisationService authService;

		public AuthoriseAttribute(string roles)
		{
			Roles = roles;
			authService = DependencyResolver.Current.GetService<IAuthorisationService>();
        }

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			var principal = filterContext.RequestContext.HttpContext.User;
			if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated && principal.Identity is ClaimsIdentity)
			{
				var result = NiceAuthorisationClaims.ProcessClaims(principal, authService, Roles.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries));

				filterContext.HttpContext.Items["UserId"] = result.IdClaim.Value;
				filterContext.HttpContext.Items["Name"] = result.Name;

				if (result.Result.Equals(NiceAuthorisationClaims.ClaimResult.Successful))
					return; //successfully verified user has role.

				if (result.Result.Equals(NiceAuthorisationClaims.ClaimResult.Forbidden))
					filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission denied"); //403

				return;
			}
			HandleUnauthorizedRequest(filterContext); //401 - which will likely result in a 302 redirect to the login page.
		}
	}
}