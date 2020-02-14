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
