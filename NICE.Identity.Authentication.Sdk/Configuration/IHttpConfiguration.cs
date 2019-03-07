using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Polly;

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IHttpConfiguration
	{
		string Token { get; }
		string Domain { get; set; }
		string Host { get; }
		string Password { get; set; }
		string UserName { get; set; }
		int HandledEventsAllowedBeforeBreaking { get; set; }
		int DurationOfBreakInMinutes { get; set; }
		Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>> CircuitBreaker { get; }
		AuthenticationHeaderValue AuthenticationHeaderValue { get; }
	}
}
