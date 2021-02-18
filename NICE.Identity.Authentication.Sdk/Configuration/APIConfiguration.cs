using Auth0.Core.Exceptions;
using Polly;
using System;
using System.Threading.Tasks;

#if NET452
using Microsoft.Owin.Logging;
#else
using Microsoft.Extensions.Logging;
#endif

namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public static class APIConfiguration
	{
		public static Polly.Retry.AsyncRetryPolicy GetAuth0RetryPolicy(ILogger _logger = null)
		{
			return Policy
				.Handle<RateLimitApiException>(ex => ex.RateLimit.Remaining < 5 && ex.RateLimit.Reset.HasValue && ex.RateLimit.Reset.Value > DateTime.UtcNow)
				.WaitAndRetryAsync(
					retryCount: 3,
					sleepDurationProvider: (retryCount, exception, context) => (((RateLimitApiException)exception).RateLimit.Reset.Value - DateTime.UtcNow),
					onRetryAsync: (exception, timespan, retryNumber, context) => 
						Task.Run(() =>
						{
							if (_logger != null)
							{
#if NET452
								_logger.WriteWarning(
									$"RateLimit for management api hit. Retry attempt no: {retryNumber}. DateTime.UtcNow: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss} " +
									$"Sleeping till: {((RateLimitApiException) exception).RateLimit.Reset.Value:dd/MM/yyyy HH:mm:ss} so sleeping for: {timespan.TotalSeconds} seconds");
#else
							_logger.LogWarning(
								$"RateLimit for management api hit. Retry attempt no: {retryNumber}. DateTime.UtcNow: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss} " +
								$"Sleeping till: {((RateLimitApiException) exception).RateLimit.Reset.Value:dd/MM/yyyy HH:mm:ss} so sleeping for: {timespan.TotalSeconds} seconds");
#endif
							}
						})
					);
		}
	}
}
