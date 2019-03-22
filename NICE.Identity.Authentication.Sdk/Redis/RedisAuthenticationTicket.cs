namespace NICE.Identity.Authentication.Sdk.Redis
{
    internal class RedisAuthenticationTicket
    {
        public string TicketValue { get; set; }
        public string Key { get; set; }
    }
}