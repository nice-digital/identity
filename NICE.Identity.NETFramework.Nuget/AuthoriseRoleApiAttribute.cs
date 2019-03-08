using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using NICE.Identity.Core.Authorisation;

namespace NICE.Identity.NETFramework.Nuget
{
	/// <summary>
	/// The AuthorizeAttribute in the System.Web.Http namespace is for use in ApiController's
	/// 
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
		

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var authService = new AuthorisationApiService(ConfigurationManager.AppSettings["AuthorisationAPIBaseUrl"]);

			var principal = actionContext.RequestContext.Principal;
			if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated && principal.Identity is ClaimsIdentity)
			{
				var claims = ((ClaimsIdentity)principal.Identity).Claims;

				var idClaim = claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier));
				if (idClaim != null)
				{
					actionContext.Request.Properties["UserId"] = idClaim.Value;
					actionContext.Request.Properties["Name"] = principal.Identity.Name;

					var rolesRequested = Roles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

					if (!rolesRequested.Any() || authService.UserSatisfiesAtLeastOneRole(idClaim.Value, rolesRequested))
					{
						return; //successfully verified user has role.
					}

					actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Permission denied"); //403
					return;
				}
			}

			HandleUnauthorizedRequest(actionContext); //401 - which will likely result in a 302 redirect to the login page.
		}
	}
}