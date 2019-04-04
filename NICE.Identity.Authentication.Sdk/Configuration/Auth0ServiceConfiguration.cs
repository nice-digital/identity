using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Polly;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public class Auth0ServiceConfiguration : IAuth0Configuration, IHttpConfiguration
	{
		public string Token => null;
		public string Domain { get; set; }
		public string Host => $"https://{Domain}/";
		public string Password{ get; set; }
		public string UserName{ get; set; }
		public int HandledEventsAllowedBeforeBreaking{ get; set; }
		public int DurationOfBreakInMinutes{ get; set; }

		public Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>> CircuitBreaker => builder => builder.CircuitBreakerAsync(
			HandledEventsAllowedBeforeBreaking,
			TimeSpan.FromMinutes(DurationOfBreakInMinutes));

		public AuthenticationHeaderValue AuthenticationHeaderValue => null;

		public string ClientId { get; set; }
		public string GrantType { get; set; }
		public string ClientSecret { get; set; }
		public string ApiIdentifier{ get; set; }
	}
}
