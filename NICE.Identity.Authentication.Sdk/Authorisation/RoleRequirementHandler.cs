using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

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

			var rolesRequired = requirement.Role.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(role => role.Trim());

			if (context.User.Claims.Any(claim =>    claim.Type.Equals(ClaimType.Role) &&
	                                                rolesRequired.Contains(claim.Value, StringComparer.OrdinalIgnoreCase)))
	        {
		        context.Succeed(requirement);
			}

			return Task.CompletedTask;
        }
    }
}