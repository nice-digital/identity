using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Claim = System.Security.Claims.Claim;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
	public class RoleRequirementHandlerTests : TestBase
    {
        private const string UserIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private readonly Mock<IAuthorisationService> _authServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly RoleRequirementHandlerDecorator _sut;

        public RoleRequirementHandlerTests()
        {
            _authServiceMock = new Mock<IAuthorisationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			_sut = new RoleRequirementHandlerDecorator(_authServiceMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public void RequirementMarkedAsSucceededWhenUserHasRole()
        {
			//Arrange
			var host = "www.nice.org.uk";
			const string roleName = Policies.Web.Administrator;
            const string userId = "auth0|user1234";

            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(UserIdClaimType, userId, host)
                })
            });

            _authServiceMock.Setup(x => x.UserSatisfiesAtLeastOneRoleForAGivenHost(userId, new[] {roleName}, host))
                            .Returns(Task.FromResult(true));

            var roleRequirement = new RoleRequirement(roleName);

            AuthorizationHandlerContext authContext = new AuthorizationHandlerContext(
                new[] {roleRequirement}, claimsPrincipal, null);

            //Act
            _sut.HandleRequirementAsync(authContext, roleRequirement);

            //Assert
            authContext.HasSucceeded.ShouldBeTrue();
            authContext.HasFailed.ShouldBeFalse();
        }

        [Fact]
        public void RequirementNotMarkedAsSucceededWhenUserDoesNotHaveRole()
        {
			//Arrange
			var host = "www.nice.org.uk";
			const string roleName = Policies.Web.Administrator;
            const string userId = "auth0|user1234";

            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(UserIdClaimType, userId, host)
                })
            });

            _authServiceMock.Setup(x => x.UserSatisfiesAtLeastOneRoleForAGivenHost(userId, new[] { roleName }, host))
                .Returns(Task.FromResult(false));

            var roleRequirement = new RoleRequirement(roleName);

            AuthorizationHandlerContext authContext = new AuthorizationHandlerContext(
                new[] { roleRequirement }, claimsPrincipal, null);

            //Act
            _sut.HandleRequirementAsync(authContext, roleRequirement);

            //Assert
            authContext.HasSucceeded.ShouldBeFalse();
        }
    }
}