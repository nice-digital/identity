using Moq;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Test.Infrastructure;
using Shouldly;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.External;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
	public class AuthorisationApiServiceTests : TestBase
	{
		private const string AuthorisationServiceUri = "https://someurl.com";
        private const string TenantDomain = "https://someurl.com";

		private readonly Mock<IHttpClientDecorator> _httpClientMock;
		private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly AuthorisationApiService _sut;

		public AuthorisationApiServiceTests()
		{
			var config = new AuthConfiguration(TenantDomain, "", "", "", "", "", AuthorisationServiceUri);

			var configOptionsMock = new Mock<IAuthConfiguration>();
            configOptionsMock.Setup(x => x.WebSettings).Returns(config.WebSettings);
            configOptionsMock.Setup(x => x.TenantDomain).Returns(config.TenantDomain);
            _httpClientMock = new Mock<IHttpClientDecorator>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
			_sut = new AuthorisationApiService(configOptionsMock.Object, _httpClientMock.Object, _httpContextAccessorMock.Object);
		}

		[Fact]
		public async Task ReturnsTrueWhenUserHasAtLeastOneMatchingRole()
		{
			//Arrange
			string[] requiredRoles = { Policies.Web.Administrator, Policies.Web.Editor };
			var host = "www.nice.org.uk";
			string userId = "auth0|user1234";
			string url = $"{AuthorisationServiceUri}{string.Format(Constants.AuthorisationURLs.GetClaims, userId)}";

			var userRoles = new[]
			{
				new Identity.Authentication.Sdk.Domain.Claim("Role", Policies.Web.Administrator, host),
				new Identity.Authentication.Sdk.Domain.Claim("FirstName", "User", host)
			};

			var authorisationApiResponse = JsonConvert.SerializeObject(userRoles);
			_httpClientMock.Setup(x => x.GetStringAsync(new Uri(url))).Returns(Task.FromResult(authorisationApiResponse));

			//Act
			var result = await _sut.UserSatisfiesAtLeastOneRoleForAGivenHost(userId, requiredRoles, host);

			//Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public async Task ReturnsFalseWhenUserHasNoMatchingRole()
		{
			//Arrange
			string[] requiredRoles = { Policies.Web.Administrator, "SomeRole1" };
			var host = "www.nice.org.uk";
			string userId = "auth0|user1234";
			string url = $"{AuthorisationServiceUri}{string.Format(Constants.AuthorisationURLs.GetClaims, userId)}";

			var userRoles = new[]
			{
				new Identity.Authentication.Sdk.Domain.Claim("Role", Policies.Web.Editor, host),
				new Identity.Authentication.Sdk.Domain.Claim("FirstName", "User", host)
			};

			var authorisationApiResponse = JsonConvert.SerializeObject(userRoles);
			_httpClientMock.Setup(x => x.GetStringAsync(new Uri(url))).Returns(Task.FromResult(authorisationApiResponse));

			//Act
			var result = await _sut.UserSatisfiesAtLeastOneRoleForAGivenHost(userId, requiredRoles, host);

			//Assert
			result.ShouldBeFalse();
		}
	}
}