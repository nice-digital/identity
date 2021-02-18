using Auth0.AuthenticationApi.Models;
using Moq;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.TokenStore;
using NICE.Identity.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
	[Trait("Category","Unit")]
    public class ApiTokenClientTests
    {
        private readonly Mock<IApiTokenStore> _apiTokenStoreMock;
        private readonly IAuthConfiguration _authConfiguration;
        private readonly JwtToken _jwtTokenResponseMock1;
        private readonly JwtToken _jwtTokenResponseMock2;
        private const string AccessToken1 = "ApiTokenTests1";
        private const string AccessToken2 = "ApiTokenTests2";

        public ApiTokenClientTests()
        {
            _jwtTokenResponseMock1 = new JwtToken {AccessToken = AccessToken1, ExpiresIn = 200, TokenType = "Bearer"};
            _jwtTokenResponseMock2 = new JwtToken { AccessToken = AccessToken2, ExpiresIn = 200, TokenType = "Bearer" };

            // mock token store
            _apiTokenStoreMock = new Mock<IApiTokenStore>();
            
            _apiTokenStoreMock.Setup(ats => 
                ats.RetrieveAsync(It.Is<string>(s => s.Equals("token store key 1")))).ReturnsAsync(_jwtTokenResponseMock1);

            _apiTokenStoreMock.Setup(ats =>
	            ats.RetrieveAsync(It.Is<string>(s => s.Equals("token store key 2")))).ReturnsAsync(_jwtTokenResponseMock2);

            _apiTokenStoreMock.Setup(ats => 
                ats.StoreAsync(It.Is<JwtToken>(token => token.AccessToken.Equals(AccessToken1)))).ReturnsAsync("token store key 1");

            _apiTokenStoreMock.Setup(ats =>
	            ats.StoreAsync(It.Is<JwtToken>(token => token.AccessToken.Equals(AccessToken2)))).ReturnsAsync("token store key 2");

            // test auth configuration 
            _authConfiguration = new AuthConfiguration(
                "tenantDomain", "clientId", "clientSecret", "redirectUri",  
                "postLogoutRedirectUri", "apiIdentifier", "authorisationServiceUri",
                "googleTrackingId", "grantType", "callBackPath", null, 
                "loginPath", "logoutPath", true, "localhost:6379");
        }

        [Fact]
        public async Task GetAccessToken()
        {
            // Arrange
            var fakeAuthenticationConnection = FakeAuthenticationConnection.Get(new AccessTokenResponse {AccessToken = _jwtTokenResponseMock1.AccessToken});
            var tokenClient = new ApiTokenClient(_apiTokenStoreMock.Object, fakeAuthenticationConnection); 
            // Act
            var accessToken = await tokenClient.GetAccessToken(_authConfiguration);
            // Assert
            Assert.Equal(_jwtTokenResponseMock1.AccessToken, accessToken);
            Assert.Equal(1, fakeAuthenticationConnection.HitCount);
        }

        [Fact]
        public async Task GetAccessTokenTwiceShouldOnlyHitTheAuthenticationAPIOnce()
        {
	        // Arrange
	        var fakeAuthenticationConnection = FakeAuthenticationConnection.Get(new AccessTokenResponse { AccessToken = _jwtTokenResponseMock1.AccessToken });
	        var tokenClient = new ApiTokenClient(_apiTokenStoreMock.Object, fakeAuthenticationConnection);
	        // Act
	        var accessToken = await tokenClient.GetAccessToken(_authConfiguration);
	        var accessToken2 = await tokenClient.GetAccessToken(_authConfiguration);
            // Assert
            Assert.Equal(_jwtTokenResponseMock1.AccessToken, accessToken);
            Assert.Equal(_jwtTokenResponseMock1.AccessToken, accessToken2);
            Assert.Equal(1, fakeAuthenticationConnection.HitCount);
        }

		[Fact]
		public async Task GetAccessTokenWithDifferentClientIdsReturnsHitsTheAPITwice()
		{
			// Arrange
			var fakeAuthenticationConnection = FakeAuthenticationConnection.Get(new AccessTokenResponse { AccessToken = _jwtTokenResponseMock1.AccessToken });
			var tokenClient = new ApiTokenClient(_apiTokenStoreMock.Object, fakeAuthenticationConnection);
			// Act
			await tokenClient.GetAccessToken(_authConfiguration);
			await tokenClient.GetAccessToken(_authConfiguration);
			await tokenClient.GetAccessToken(_authConfiguration.TenantDomain, "different client id", _authConfiguration.WebSettings.ClientSecret, _authConfiguration.MachineToMachineSettings.ApiIdentifier);
            // Assert
			Assert.Equal(2, fakeAuthenticationConnection.HitCount);
        }
	}
}