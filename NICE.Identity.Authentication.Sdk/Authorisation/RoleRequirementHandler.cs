using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly IHttpClientFactory _httpClientFactory;
	    private readonly IAuthConfiguration _authConfiguration;

	    public RoleRequirementHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IAuthConfiguration authConfiguration)
	    {
		    _httpContextAccessor = httpContextAccessor;
		    _httpClientFactory = httpClientFactory;
		    _authConfiguration = authConfiguration;
	    }

	    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
			if (!context.User.Identity.IsAuthenticated)
			{
				return;
	        }

			//if the user doesn't have any idam claims, then add them. this will happen during M2M auth.
			if (!context.User.Claims.Any(claim => claim.Type.Equals(ClaimType.IdAMId)))
			{
				var userId = context.User.Claims.Single(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
				var authorisationHeader = _httpContextAccessor.HttpContext.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
				var authHeader = AuthenticationHeaderValue.Parse(authorisationHeader);
				var client = _httpClientFactory.CreateClient();

				await ClaimsHelper.AddClaimsToUser(_authConfiguration, userId, authHeader.Parameter, _httpContextAccessor.HttpContext.Request.Host.Host, context.User, client);
			}

			var rolesRequired = requirement.Role.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(role => role.Trim());

			if (context.User.Claims.Any(claim => claim.Type.Equals(ClaimType.Role) &&
			                                     rolesRequired.Contains(claim.Value, StringComparer.OrdinalIgnoreCase)))
			{
		        context.Succeed(requirement);
			}
        }
    }
}