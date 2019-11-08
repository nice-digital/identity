using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Authorisation;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
    public class RoleRequirementHandlerDecorator : RoleRequirementHandler
    {
        public RoleRequirementHandlerDecorator(IAuthorisationService authorisationService, IHttpContextAccessor httpContextAccessor) : base(authorisationService, httpContextAccessor)
        {
        }

        public Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            return base.HandleRequirementAsync(context, requirement);
        }
    }
}