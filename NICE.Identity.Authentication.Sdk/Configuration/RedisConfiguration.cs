namespace NICE.Identity.Authentication.Sdk.Configuration
{
    public class RedisConfiguration
    {
        public string IpConfig { get; set; }

        public int Port { get; set; }

        public string ConnectionString => $"{IpConfig}:{Port}";
    }
}
