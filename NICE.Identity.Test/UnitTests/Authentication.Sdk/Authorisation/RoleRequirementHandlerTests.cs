﻿using Microsoft.AspNetCore.Authorization;
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
		private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly RoleRequirementHandlerDecorator _sut;

		public RoleRequirementHandlerTests()
		{
			_httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			_sut = new RoleRequirementHandlerDecorator();
		}

		[Fact]
		public void RequirementMarkedAsSucceededWhenUserHasRole()
		{
			//Arrange
			var host = "www.nice.org.uk";
			const string roleName = Policies.Web.Administrator;

			var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
			{
				new ClaimsIdentity(new List<Claim>()
				{
					new Claim(ClaimTypes.NameIdentifier, "auth0|user1234", host),
					new Claim(ClaimTypes.Role, "another role", host),
					new Claim(ClaimTypes.Role, roleName, host),
					new Claim(ClaimTypes.Role, "yet another role", host)
				}, authenticationType: "IdAM")
			});
			

			var roleRequirement = new RoleRequirement(roleName);

			AuthorizationHandlerContext authContext = new AuthorizationHandlerContext(
				new[] { roleRequirement }, claimsPrincipal, null);

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
			
			var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
			{
				new ClaimsIdentity(new List<Claim>()
				{
					new Claim(ClaimTypes.NameIdentifier, "auth0|user1234", host)
				}, authenticationType: "IdAM")
			});

			var roleRequirement = new RoleRequirement(roleName);

			AuthorizationHandlerContext authContext = new AuthorizationHandlerContext(
				new[] { roleRequirement }, claimsPrincipal, null);

			//Act
			_sut.HandleRequirementAsync(authContext, roleRequirement);

			//Assert
			authContext.HasSucceeded.ShouldBeFalse();
		}

		[Fact]
		public void RequirementNotMarkedAsSucceededWhenUserIsNotAuthenticated()
		{
			//Arrange
			var host = "www.nice.org.uk";
			const string roleName = Policies.Web.Administrator;
			
			var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
			{
				new ClaimsIdentity(new List<Claim>()
				{
					new Claim(ClaimTypes.NameIdentifier, "auth0|user1234", host)
				}) //not setting authenticationType results in this user not being authenticated.
			});

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