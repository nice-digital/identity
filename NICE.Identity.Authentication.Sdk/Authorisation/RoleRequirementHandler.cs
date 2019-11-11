using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
	        if (!context.User.Identity.IsAuthenticated)
	        {
				return Task.CompletedTask;
	        }

	        if (context.User.Claims.Any(claim =>    claim.Type.Equals(ClaimType.Role) &&
													claim.Value.Equals(requirement.Role, StringComparison.OrdinalIgnoreCase)))
	        {
		        context.Succeed(requirement);
			}

			return Task.CompletedTask;
        }
    }
}