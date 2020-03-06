using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Tracking
{
	public static class TrackingService
	{
		private static readonly string endpoint = "http://www.google-analytics.com/collect";
		private static readonly string googleVersion = "1";
		private static readonly string googleCookieName = "_ga";

		/// <summary>
		/// this uses the google measurement protocol to track the sign in:
		/// https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide#page
		///
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		public static void TrackSuccessfulSignIn(HttpClient httpClient, Dictionary<string, string> cookies, string trackingId)
		{
			if (string.IsNullOrEmpty(trackingId))
				throw new ArgumentNullException(nameof(trackingId));

			if (cookies == null)
				throw new ArgumentNullException(nameof(cookies));

			if (!cookies.ContainsKey(googleCookieName))
				throw new ArgumentException($"Cookie {googleCookieName} not found");
			var gaCookie = cookies[googleCookieName];

			var gaCookieRegex = new Regex(@"^GA\d\.\d\."); //for getting the client id. see: https://stackoverflow.com/questions/31854752/how-to-get-the-client-id-while-sending-data-to-ga-using-measurement-protocol
			var clientId = gaCookieRegex.Replace(gaCookie, string.Empty);

			TrackEvent(httpClient, trackingId, clientId, "todo: category", "todo: action", "todo: label");
		}

		public static async Task<HttpResponseMessage> TrackEvent(HttpClient httpClient, string trackingId, string googleClientId, string category, string action, string label, int? value = null)
		{
			if (string.IsNullOrEmpty(category))
				throw new ArgumentNullException(nameof(category));

			if (string.IsNullOrEmpty(action))
				throw new ArgumentNullException(nameof(action));

			var postData = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("v", googleVersion),
				new KeyValuePair<string, string>("tid", trackingId),
				new KeyValuePair<string, string>("cid", googleClientId),
				new KeyValuePair<string, string>("t", "event"),
				new KeyValuePair<string, string>("ec", category),
				new KeyValuePair<string, string>("ea", action)
			};

			if (label != null)
			{
				postData.Add(new KeyValuePair<string, string>("el", label));
			}

			if (value != null)
			{
				postData.Add(new KeyValuePair<string, string>("ev", value.ToString()));
			}

			return await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(postData)).ConfigureAwait(false);
		}
	}
}