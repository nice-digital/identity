using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        private const string UserIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private readonly IAuthorisationService _authorisationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleRequirementHandler(IAuthorisationService authorisationService, IHttpContextAccessor httpContextAccessor)
        {
	        _authorisationService = authorisationService ?? throw new ArgumentNullException(nameof(authorisationService));
	        _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type.Equals(UserIdClaimType)) ||
                context.User.Claims.Single(x => x.Type.Equals(UserIdClaimType)).Value == null)
            {
                return Task.CompletedTask;
            }

            var userId = context.User.Claims.Single(x => x.Type.Equals(UserIdClaimType)).Value;

            var host = _httpContextAccessor.HttpContext.Request.Host.Host;

            // TODO: Add claims to user either on login or when calling the claims endpoint for the first time.
            var userIsAuthorised = _authorisationService.UserSatisfiesAtLeastOneRoleForAGivenHost(userId, new[] {requirement.Role}, host).Result;

            if (userIsAuthorised)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}