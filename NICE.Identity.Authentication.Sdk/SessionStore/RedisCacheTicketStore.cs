#if NETSTANDARD2_0 || NETCOREAPP3_1
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace NICE.Identity.Authentication.Sdk.SessionStore
{
    public class RedisCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "IdAM-";
        private IDistributedCache _cache;

        public RedisCacheTicketStore(RedisCacheOptions options)
        {
            _cache = new RedisCache(options);
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString();
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new DistributedCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }
            byte[] val = SerializeToBytes(ticket);
            _cache.Set(key, val, options);
            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            AuthenticationTicket ticket;
            byte[] bytes = null;
            bytes = _cache.Get(key);
            ticket = DeserializeFromBytes(bytes);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }

        private static byte[] SerializeToBytes(AuthenticationTicket source)
        {
            return TicketSerializer.Default.Serialize(source);
        }

        private static AuthenticationTicket DeserializeFromBytes(byte[] source)
        {
            return source == null ? null : TicketSerializer.Default.Deserialize(source);
        }
    }
}
#endif