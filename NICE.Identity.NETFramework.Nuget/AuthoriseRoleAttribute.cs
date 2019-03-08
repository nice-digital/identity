using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using NICE.Identity.Core.Authorisation;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace NICE.Identity.NETFramework.Nuget
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Mvc namespace is for use in Web Controllers 
	/// </summary>
	public class AuthoriseAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		public AuthoriseAttribute()
		{}
		public AuthoriseAttribute(string roles)
		{
			this.Roles = roles;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			var authService = new AuthorisationApiService(ConfigurationManager.AppSettings["AuthorisationAPIBaseUrl"]);

			var principal = filterContext.RequestContext.HttpContext.User;
			if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated && principal.Identity is ClaimsIdentity)
			{
				var claims = ((ClaimsIdentity)principal.Identity).Claims;

				var idClaim = claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier));
				if (idClaim != null)
				{
					filterContext.HttpContext.Items["UserId"] = idClaim.Value;
					filterContext.HttpContext.Items["Name"] = principal.Identity.Name;

					var rolesRequested = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

					if (!rolesRequested.Any() || authService.UserSatisfiesAtLeastOneRole(idClaim.Value, rolesRequested))
					{
						return; //successfully verified user has role.
					}

					filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Permission denied"); //403
					return;
				}
			}
			HandleUnauthorizedRequest(filterContext); //401 - which will likely result in a 302 redirect to the login page.
		}
	}
}