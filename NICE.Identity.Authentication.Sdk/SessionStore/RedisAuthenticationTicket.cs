﻿namespace NICE.Identity.Authentication.Sdk.SessionStore
{
    internal class RedisAuthenticationTicket
    {
        public string TicketValue { get; set; }
        public string Key { get; set; }
    }
}