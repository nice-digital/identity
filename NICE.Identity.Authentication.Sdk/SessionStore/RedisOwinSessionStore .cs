using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NICE.Identity.Authentication.Sdk.SessionStore
{
    public class RedisOwinSessionStore : IAuthenticationSessionStore
    {
        private readonly ICacheClient _cacheClient;
        private readonly JsonSerializerSettings jsonSetting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private TicketDataFormat _formatter;

        public RedisOwinSessionStore(TicketDataFormat formatter, RedisConfiguration redisConfiguration)
        {
            var serializer = new NewtonsoftSerializer(jsonSetting);
            var mux = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                DefaultVersion = new Version(3, 0, 503),
                EndPoints = {{redisConfiguration.IpConfig, redisConfiguration.Port}},
                AllowAdmin = true,
                ConnectTimeout = 5000,
                Ssl = false
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
            var redisAuthenticationTicket = await _cacheClient.GetAsync<RedisAuthenticationTicket>(key);

	        if (redisAuthenticationTicket == null)
		        return null;

			var result = _formatter.Unprotect(redisAuthenticationTicket.TicketValue);

			if (result == default(AuthenticationTicket))
				return null;

			return result;

        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var ticketData = new RedisAuthenticationTicket
            {
                TicketValue = _formatter.Protect(ticket),
                Key = $"{Guid.NewGuid().ToString()}.Id-Keys"

            };

            await _cacheClient.AddAsync(ticketData.Key, ticketData, TimeSpan.FromSeconds(3600));
            return ticketData.Key;
        }

    }
}
