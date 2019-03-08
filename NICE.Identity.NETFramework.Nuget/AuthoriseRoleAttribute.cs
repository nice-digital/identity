using System;
using Microsoft.Extensions.Options;
using NICE.Identity.NETFramework.Nuget.Abstractions;
using NICE.Identity.NETFramework.Nuget.Authorisation;
using NICE.Identity.NETFramework.Nuget.External;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Mvc;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace NICE.Identity.NETFramework.Nuget
{
	public class AuthoriseRoleAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);

			if (string.IsNullOrEmpty(Roles))
				return; //no roles specified. just let it through.

			var config = new OptionsWrapper<AuthorisationServiceConfiguration>(new AuthorisationServiceConfiguration
			{
				BaseUrl = ConfigurationManager.AppSettings["AuthorisationAPIBaseUrl"]
			});
			var httpClient = new HttpClientDecorator(new HttpClient()); //TODO: figure out how to avoid new'ing up a new HttpClient here.
			var authService = new AuthorisationApiService(config, httpClient);

			var principal = filterContext.RequestContext.HttpContext.User;
			if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated && principal.Identity is ClaimsIdentity)
			{
				var claims = ((ClaimsIdentity)principal.Identity).Claims;

				var idClaim = claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier));
				if (idClaim != null)
				{
					var rolesRequested = Roles.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);

					if (authService.UserSatisfiesAtLeastOneRole(idClaim.Value, rolesRequested).Result) //TODO: async
					{
						return; //successfully verified user has role.
					}
				}
			}

			HandleUnauthorizedRequest(filterContext);
		}
	}
}