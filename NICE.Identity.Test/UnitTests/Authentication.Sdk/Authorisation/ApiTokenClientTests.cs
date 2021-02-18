using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.TokenStore;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using Xunit;

namespace NICE.Identity.Test.UnitTests.Authentication.Sdk.Authorisation
{
    [Trait("Category","Unit")]
    public class ApiTokenClientTests
    {
        private readonly Mock<IApiTokenStore> _apiTokenStoreMock;
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly IAuthConfiguration _authConfiguration;
        private readonly JwtToken _jwtTokenResponseMock;

        public ApiTokenClientTests()
        {
            _jwtTokenResponseMock = new JwtToken {AccessToken = "ApiTokenTests", ExpiresIn = 200, TokenType = "Bearer"};

            // mock token store
            _apiTokenStoreMock = new Mock<IApiTokenStore>();
            _apiTokenStoreMock.Setup(ats => 
                ats.RetrieveAsync(It.IsAny<string>())).ReturnsAsync(_jwtTokenResponseMock);
            _apiTokenStoreMock.Setup(ats => 
                ats.StoreAsync(It.IsAny<JwtToken>())).ReturnsAsync(_jwtTokenResponseMock.AccessToken);

            // mock http client factory with token response
            var httpMessageHandlerMock = new Mock<DelegatingHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ObjectContent<JwtToken>(_jwtTokenResponseMock, new JsonMediaTypeFormatter())
                })
                .Verifiable();
            httpMessageHandlerMock.As<IDisposable>().Setup(s => s.Dispose());
            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _clientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            _clientFactoryMock.Setup(cf => cf.CreateClient(It.IsAny<string>())).Returns(httpClient).Verifiable();

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
            var tokenClient = new ApiTokenClient(_apiTokenStoreMock.Object); 
            // Act
            var accessToken = await tokenClient.GetAccessToken(_authConfiguration);
            // Assert
            Assert.Equal(_jwtTokenResponseMock.AccessToken, accessToken);
        }
    }
}