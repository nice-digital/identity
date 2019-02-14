using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.Abstractions;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        private const string UserIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private readonly IAuthorisationService _authorisationService;

        public RoleRequirementHandler(IAuthorisationService authorisationService)
        {
            _authorisationService =
                authorisationService ?? throw new ArgumentNullException(nameof(authorisationService));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type.Equals(UserIdClaimType)) ||
                context.User.Claims.Single(x => x.Type.Equals(UserIdClaimType)).Value == null)
            {
                return Task.CompletedTask;
            }

            var userId = context.User.Claims.Single(x => x.Type.Equals(UserIdClaimType)).Value;

            var userIsAuthorised = _authorisationService.UserSatisfiesAtLeastOneRole(userId, new[] {requirement.Role}).Result;

            if (userIsAuthorised)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}