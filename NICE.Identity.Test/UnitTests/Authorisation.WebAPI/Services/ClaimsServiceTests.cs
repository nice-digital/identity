using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System.Linq;
using System.Security.Claims;
using Xunit;
using Claim = NICE.Identity.Authorisation.WebAPI.ApiModels.Claim;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
	public class ClaimsServiceTests : TestBase
    {
        private readonly Mock<ILogger<ClaimsService>> _logger;
        private IdentityContext _identityContext;
        private readonly IUsersService _usersService;

		private ClaimsService _sut;

        public ClaimsServiceTests()
        {
            _logger = new Mock<ILogger<ClaimsService>>();
            _usersService = new MockUserService();
			_identityContext = GetContext();

            _sut = new ClaimsService(_identityContext, _logger.Object, _usersService);
        }

        [Fact]
        public void UserIsReturnedInClaims()
        {
            //Arrange
            TestData.AddWithTwoRoles(ref _identityContext);

            //Act
            var claims = _sut.GetClaims("some auth0 userid");

            //Assert
            claims.Single(claim => claim.Type.Equals(ClaimType.FirstName)).Value.ShouldBe("Steve");
            claims.Single(claim => claim.Type.Equals(ClaimType.LastName)).Value.ShouldBe("Zissou");
        }

        [Fact]
        public void UserIsNotRegistered()
        {
            //Arrange

            //Act
            var result = _sut.GetClaims("some auth0 userid");

            //Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void UserDoesNotHaveRoles()
        {
            //Arrange
            TestData.AddUserNoRole(ref _identityContext);

            //Act
            var claims = _sut.GetClaims("some auth0 userid");

            //Assert
            claims.Single(claim => claim.Type.Equals(ClaimType.FirstName)).Value.ShouldBe("Steve");
            claims.Single(claim => claim.Type.Equals(ClaimType.LastName)).Value.ShouldBe("Zissou");
		}

        [Fact]
        public void OrganisationIsSerialisedAndDeserialised()
        {
	        //Arrange
	        var leadOrganisationName = "Belafonte";
	        TestData.AddWithTwoJobsOneLead(ref _identityContext, leadOrganisationName);

	        //Act
	        var claims = _sut.GetClaims("some auth0 userid");
	        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(claim => new System.Security.Claims.Claim(claim.Type, claim.Value, "www.nice.org.uk", claim.Issuer))));
	        var organisations = NICE.Identity.Authentication.Sdk.Extensions.ClaimsExtensions.OrganisationsAssignedAsLead(claimsPrincipal);

            //Assert
            organisations.Single().OrganisationName.Equals(leadOrganisationName);
        }
    }
}
