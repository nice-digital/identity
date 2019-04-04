using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Abstractions;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.External;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
    public class AuthorisationApiServiceTests : TestBase
    {
        private const string BaseUrl = "https://someurl.com";

        private readonly Mock<IHttpClientDecorator> _httpClientMock;
        private readonly AuthorisationApiService _sut;

        public AuthorisationApiServiceTests()
        {
            var config = new AuthorisationServiceConfiguration
            {
                BaseUrl = BaseUrl
            };

            var configOptionsMock = new Mock<IOptions<AuthorisationServiceConfiguration>>();
            configOptionsMock.Setup(x => x.Value).Returns(config);

            _httpClientMock = new Mock<IHttpClientDecorator>();
            _httpClientMock.Setup(x => x.BaseUrl).Returns(new Uri(config.BaseUrl));
            
            _sut = new AuthorisationApiService(_httpClientMock.Object);
        }

        [Fact]
        public async Task ReturnsTrueWhenUserHasAtLeastOneMatchingRole()
        {
            //Arrange
            string[] requiredRoles = { PolicyTypes.Administrator, PolicyTypes.Editor };
            string userId = "auth0|user1234";
            string url = $"{BaseUrl}{string.Format(Constants.AuthorisationURLs.GetClaims, userId)}";

            var userRoles = new[]
            {
                new Claim("Role", PolicyTypes.Administrator),
                new Claim("FirstName", "User")
            };

            var authorisationApiResponse = JsonConvert.SerializeObject(userRoles);
            _httpClientMock.Setup(x => x.GetStringAsync(new Uri(url))).Returns(Task.FromResult(authorisationApiResponse));
            
            //Act
            var result = await _sut.UserSatisfiesAtLeastOneRole(userId, requiredRoles);

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ReturnsFalseWhenUserHasNoMatchingRole()
        {
            //Arrange
            string[] requiredRoles = { PolicyTypes.Administrator, "SomeRole1" };
            string userId = "auth0|user1234";
            string url = $"{BaseUrl}{string.Format(Constants.AuthorisationURLs.GetClaims, userId)}";

            var userRoles = new[]
            {
                new Claim("Role", PolicyTypes.Editor),
                new Claim("FirstName", "User")
            };

            var authorisationApiResponse = JsonConvert.SerializeObject(userRoles);
            _httpClientMock.Setup(x => x.GetStringAsync(new Uri(url))).Returns(Task.FromResult(authorisationApiResponse));

            //Act
            var result = await _sut.UserSatisfiesAtLeastOneRole(userId, requiredRoles);

            //Assert
            result.ShouldBeFalse();
        }
    }
}