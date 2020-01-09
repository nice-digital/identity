#if NETSTANDARD || NETCOREAPP3_1
using System;
using Microsoft.AspNetCore.Http;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class HttpRequestExtensions
	{
		private const string UnknownHostName = "UNKNOWN-HOST";
		private const string MultipleHostName = "MULTIPLE-HOST";
		private const string Comma = ",";

		/// <summary>
		/// Gets http request Uri from request object.
		/// </summary>
		/// <param name="request">The <see cref="HttpRequest"/>.</param>
		/// <returns>A New Uri object representing request Uri.</returns>
		public static Uri GetUri(this HttpRequest request, bool forceHttps = false)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			if (string.IsNullOrWhiteSpace(request.Scheme) == true)
			{
				throw new ArgumentException("Http request Scheme is not specified");
			}

			return new Uri(string.Concat(
				forceHttps ? Uri.UriSchemeHttps : request.Scheme,
				"://",
				request.Host.HasValue ? (request.Host.Value.IndexOf(Comma, StringComparison.Ordinal) > 0 ? MultipleHostName : request.Host.Value) : UnknownHostName,
				request.Path.HasValue ? request.Path.Value : string.Empty,
				request.QueryString.HasValue ? request.QueryString.Value : string.Empty));
		}
    }
}
#endif