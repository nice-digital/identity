using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.NETFramework.Authorisation;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
    public class RoleRequirementHandlerDecorator : RoleRequirementHandler
    {
        public RoleRequirementHandlerDecorator(IAuthorisationService authorisationService) : base(authorisationService)
        {
        }

        public Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            return base.HandleRequirementAsync(context, requirement);
        }
    }
}