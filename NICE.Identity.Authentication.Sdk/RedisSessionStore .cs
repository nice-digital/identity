using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Configurations;
using Rhasta.Owin.Security.Cookies.Store.Redis;
using StackExchange.Redis;

namespace NICE.Identity.Authentication.Sdk
{
    public class RedisSessionStore : IAuthenticationSessionStore
    {
        private readonly ICacheClient _cacheClient;
        private readonly JsonSerializerSettings jsonSetting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private TicketDataFormat _formatter;

        public RedisSessionStore(TicketDataFormat formatter, RedisConfiguration redisConfiguration)
        {
            var serializer = new NewtonsoftSerializer(jsonSetting);
            var mux = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                DefaultVersion = new Version(3, 0, 500),
                EndPoints = {{redisConfiguration.IpConfig, redisConfiguration.Port}},
                AllowAdmin = true
            });
            _cacheClient = new StackExchangeRedisCacheClient(mux, serializer);
            _formatter = formatter;
        }

        public async Task RemoveAsync(string key)
        {
            await _cacheClient.RemoveAsync(key);

        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var isExisted = await _cacheClient.ExistsAsync(key);
            var ticketData = new RedisAuthenticationTicket
            {
                TicketValue = _formatter.Protect(ticket),
                Key = key

            };
            if (isExisted)
            {
                await _cacheClient.ReplaceAsync(key, ticketData);
            }
            else
            {
                await _cacheClient.AddAsync(key, ticketData);
            }

        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticketData = await _cacheClient.GetAsync<RedisAuthenticationTicket>(key);

            return _formatter.Unprotect(ticketData.TicketValue);

        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var ticketData = new RedisAuthenticationTicket
            {
                TicketValue = _formatter.Protect(ticket),
                Key = Guid.NewGuid().ToString()

            };

            await _cacheClient.AddAsync(ticketData.Key, ticketData);
            return ticketData.Key;
        }

    }
}
