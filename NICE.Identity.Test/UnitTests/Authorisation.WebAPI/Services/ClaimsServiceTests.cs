﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using NICE.Identity.Authorisation.WebAPI.Services;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authorisation.WebAPI.Services
{
    public class ClaimsServiceTests : TestBase
    {
        private readonly Mock<ILogger<ClaimsService>> _logger;
        private IdentityContext _identityContext;

        private ClaimsService _sut;

        public ClaimsServiceTests()
        {
            _logger = new Mock<ILogger<ClaimsService>>();
            _identityContext = GetContext();

            _sut = new ClaimsService(_identityContext, _logger.Object);
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
        }
    }
}
