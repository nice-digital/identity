using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
	/// <summary>
	/// This decorator class is only here as the base method is marked as protected. this just changes the access to public.
	/// </summary>
    public class RoleRequirementHandlerDecorator : RoleRequirementHandler
    {
	    public RoleRequirementHandlerDecorator(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IAuthConfiguration authConfiguration) : base(httpContextAccessor, httpClientFactory, authConfiguration)
	    {
	    }

	    public new Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            return base.HandleRequirementAsync(context, requirement);
        }
    }
}