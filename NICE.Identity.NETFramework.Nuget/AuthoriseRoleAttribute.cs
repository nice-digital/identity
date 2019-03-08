using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using NICE.Identity.NETFramework.Nuget.Abstractions;
using NICE.Identity.NETFramework.Nuget.Authorisation;
using NICE.Identity.NETFramework.Nuget.External;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace NICE.Identity.NETFramework.Nuget
{
	public class AuthoriseRoleAttribute : System.Web.Http.AuthorizeAttribute //System.Web.Http.Filters.AuthorizationFilterAttribute
	{
		//public override void OnAuthorization(HttpActionContext actionContext)
		//{
		//	base.OnAuthorization(actionContext);
		//}
		//public override void OnAuthorization(HttpActionContext actionContext)
		//{
		//	base.OnAuthorization(actionContext);
		//}

		public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
		{
			return base.OnAuthorizationAsync(actionContext, cancellationToken);
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if (string.IsNullOrEmpty(Roles))
				return; //no roles specified. just let it through.

			var config = new OptionsWrapper<AuthorisationServiceConfiguration>(new AuthorisationServiceConfiguration
			{
				BaseUrl = ConfigurationManager.AppSettings["AuthorisationAPIBaseUrl"]
			});
			var httpClient = new HttpClientDecorator(new HttpClient()); //TODO: figure out how to avoid new'ing up a new HttpClient here.
			var authService = new AuthorisationApiService(config, httpClient);

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

					if (authService.UserSatisfiesAtLeastOneRole(idClaim.Value, rolesRequested).Result)
					{
						return; //successfully verified user has role.
					}
				}
			}

			HandleUnauthorizedRequest(actionContext);
		}
	}
}