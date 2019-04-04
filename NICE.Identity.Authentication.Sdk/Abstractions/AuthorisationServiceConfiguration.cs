using System;
using System.Net.Http;
using System.Net.Http.Headers;
using NICE.Identity.Authentication.Sdk.Configuration;
using Polly;

namespace NICE.Identity.Authentication.Sdk.Abstractions
{
    public class AuthorisationServiceConfiguration : IHttpConfiguration
    {
        public string BaseUrl { get; set; }

        public string Token => null;

        public string Host => BaseUrl;

        public string Password { get; set; }
        public string UserName { get; set; }
        public int HandledEventsAllowedBeforeBreaking { get; set; }
        public int DurationOfBreakInMinutes { get; set; }

        public Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>> CircuitBreaker => builder => builder.CircuitBreakerAsync(
            HandledEventsAllowedBeforeBreaking,
            TimeSpan.FromMinutes(DurationOfBreakInMinutes));

        public AuthenticationHeaderValue AuthenticationHeaderValue => null;
    }
}