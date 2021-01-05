using System;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
#if !NET452
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
#endif

namespace NICE.Identity.Authentication.Sdk.TokenStore
{
    public class RedisApiTokenStore : IApiTokenStore, IDisposable
    {
        private const string KeyPrefix = "APITOKEN-";
        #if NET452
        private readonly StackExchangeRedisCacheClient _cacheClient;
        #else
        private readonly IRedisDatabase _cacheClient;
        private readonly IRedisCacheConnectionPoolManager _connectionPoolManager;
        #endif

        #if NETSTANDARD2_0 || NETCOREAPP3_1
        public RedisApiTokenStore(IRedisCacheClient cacheClient)
        {
            _cacheClient = cacheClient.GetDbFromConfiguration();
        }
        #elif NET461 || NET472
        public RedisApiTokenStore(string redisConnectionString)
        {
            var serializer = new NewtonsoftSerializer();
            var redisConfiguration = new RedisConfiguration()
            {
                ConnectionString = redisConnectionString
            };
            _connectionPoolManager = new RedisCacheConnectionPoolManager(redisConfiguration);
            _cacheClient = new RedisCacheClient(_connectionPoolManager, serializer, redisConfiguration).GetDbFromConfiguration();
        }
        #else
        public RedisApiTokenStore(string redisConnectionString)
        {
            var serializer = new NewtonsoftSerializer();
            _cacheClient = new StackExchangeRedisCacheClient(serializer, redisConnectionString);
        }
        #endif

        public Task RemoveAsync(string key)
        {
            return Task.FromResult(_cacheClient.RemoveAsync(key));
        }

        public async Task RenewAsync(string key, JwtToken token)
        {
            await _cacheClient.ReplaceAsync(key, token, TimeSpan.FromSeconds(token.ExpiresIn));
        }

        public async Task<JwtToken> RetrieveAsync(string key)
        {
            return await _cacheClient.GetAsync<JwtToken>(key);
        }

        public async Task<string> StoreAsync(JwtToken token)
        {
            var key = $"{KeyPrefix}{Guid.NewGuid().ToString()}";
            await _cacheClient.AddAsync(key, token, TimeSpan.FromSeconds(token.ExpiresIn));
            return key;
        }

        public void Dispose()
        {
        #if NETFRAMEWORK
            var endPoints = _cacheClient.Database.Multiplexer.GetEndPoints(true);
            foreach (var endpoint in endPoints)
            {
                var server = _cacheClient.Database.Multiplexer.GetServer(endpoint);
                server.FlushDatabase();
            }
            
            #if NET461 || NET472
           _connectionPoolManager.Dispose();
            #else
           _cacheClient.Dispose();
            #endif
        #endif
        }
    }
}