#if NETSTANDARD2_0 || NETCOREAPP3_1
using System;
using System.Linq;
using System.Net;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class RedisExtensions
	{
		/// <summary>
		/// .net core 3.1 uses version 6.3.4 of StackExchange.Redis.Extensions, which has a settable connection string property.
		///
		///
		/// Unfortunately .net standard is locked to using version 5.5.0 of StackExchange.Redis.Extensions which doesn't have a connection string property
		/// .
		/// So we have to manually convert a ConfigurationOptions object (which can be parsed from a connection string) into a RedisConfiguration object.
		///
		/// A better solution might be to map the RedisConfiguration object from JSON directly. however then you'd need to specify the redis configuration differently between
		/// usages of the SDK, which would be a shame.
		/// </summary>
		/// <param name="configurationOptions"></param>
		/// <returns></returns>
		public static RedisConfiguration ToRedisConfiguration(this string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

#if NETCOREAPP3_1
			return new RedisConfiguration { ConnectionString = connectionString };
#endif

			var configurationOptions = StackExchange.Redis.ConfigurationOptions.Parse(connectionString);

			var redisConfiguration = new RedisConfiguration()
			{
				AbortOnConnectFail = configurationOptions.AbortOnConnectFail,
				AllowAdmin = configurationOptions.AllowAdmin,
				ConfigurationChannel = configurationOptions.ConfigurationChannel,
				ConnectTimeout = configurationOptions.ConnectTimeout,
				Database = configurationOptions.DefaultDatabase ?? 0,
				Ssl = configurationOptions.Ssl,
				Password = configurationOptions.Password,
				SyncTimeout = configurationOptions.SyncTimeout,

				Hosts = configurationOptions.EndPoints.Select(endpoint =>
				{
					var ipEndPoint = ParseIPEndPoint(endpoint.ToString());
					return new RedisHost { Host = ipEndPoint.Address.ToString(), Port = ipEndPoint.Port };
				}).ToArray()
			};

			return redisConfiguration;
		}

		private static IPEndPoint ParseIPEndPoint(string text)
		{
			Uri uri;
			if (Uri.TryCreate(text, UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			throw new FormatException("Failed to parse text to IPEndPoint");
		}

	}
}
#endif