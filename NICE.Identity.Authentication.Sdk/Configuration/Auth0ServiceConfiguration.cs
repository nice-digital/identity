using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Polly;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public class Auth0ServiceConfiguration : IAuth0Configration, IHttpConfiguration
	{
		public string Token => throw new NotImplementedException();

		public string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int HandledEventsAllowedBeforeBreaking { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int DurationOfBreakInMinutes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>> CircuitBreaker => throw new NotImplementedException();

		public AuthenticationHeaderValue AuthenticationHeaderValue => throw new NotImplementedException();

		public string ClientId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string GrantType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ClientSecret { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ApiIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
