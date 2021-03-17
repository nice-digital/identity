using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.Authentication.Sdk.TokenStore;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NICE.Identity.Test.IntegrationTests.Authentication.Sdk.TokenStore
{
	[Trait("Category","Integration")]
    public class ApiTokenStoreTests : IDisposable
    {
        private readonly IRedisCacheConnectionPoolManager _connectionPoolManager;
        private readonly IApiTokenStore _apiTokenStore;

        public ApiTokenStoreTests()
        {
            var serializer = new NewtonsoftSerializer(new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var redisConfiguration = RedisExtensions.ToRedisConfiguration("127.0.0.1:6379,allowAdmin=true");

            _connectionPoolManager = new RedisCacheConnectionPoolManager(redisConfiguration);
            var cacheClient = new RedisCacheClient(_connectionPoolManager, serializer, redisConfiguration);
            _apiTokenStore = new RedisApiTokenStore(cacheClient);
        }

        public void Dispose()
        {
            _connectionPoolManager.Dispose();
        }

        [Fact]
        public async Task StoreAndRetrieveApiToken()
        {
            // Arrange
            var token = new JwtToken {AccessToken = "ApiTokenStoreTests", ExpiresIn = 60, TokenType = "Bearer"};
            // Act
            var tokenKey = await _apiTokenStore.StoreAsync(token);
            var tokenStored = _apiTokenStore.RetrieveAsync(tokenKey);
            // Assert
            Assert.Equal(token.AccessToken, tokenStored.Result.AccessToken);
        }
    }
}