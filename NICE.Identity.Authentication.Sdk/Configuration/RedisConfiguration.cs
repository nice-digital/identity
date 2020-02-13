#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.Extensions.Configuration;
#endif

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IRedisConfiguration
	{
		string IpConfig { get; }
		int Port { get; }
		bool Enabled { get; }

		/// <summary>
		/// optional. if not set, defaults to "[ip]:[port]"
		/// </summary>
		string ConnectionString { get; set; }
    }

	public class RedisConfiguration : IRedisConfiguration
	{
#if NETSTANDARD2_0 || NETCOREAPP3_1
		public RedisConfiguration(IConfiguration configuration, string appSettingsSectionName)
		{
			var section = configuration.GetSection(appSettingsSectionName);
			IpConfig = section["IpConfig"];
			Port = int.TryParse(section["Port"], out var port) ? port : 6379;
			Enabled = !string.IsNullOrEmpty(IpConfig) && bool.TryParse(section["Enabled"], out var enabled) ? enabled : false;
		}
#endif
		public RedisConfiguration(string ipConfig, int port, bool enabled, string connectionString = null)
		{
			IpConfig = ipConfig;
			Port = port;
			Enabled = enabled;
			if (!string.IsNullOrEmpty(connectionString))
			{
				ConnectionString = connectionString;
			}
		}

		public string IpConfig { get; private set; }
		public int Port { get; private set; }

		public bool Enabled { get; private set; }

		private string _connectionString;
		public string ConnectionString
		{
			get => _connectionString ?? $"{IpConfig}:{Port}"; 
			set => _connectionString = value;
		}
	}
}
