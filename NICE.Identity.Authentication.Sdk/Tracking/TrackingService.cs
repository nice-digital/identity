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
		private static readonly string googleTypeEvent = "event";
		private static readonly string googleTypePageview = "pageview";

		/// <summary>
		/// this uses the google measurement protocol to track the sign in:
		/// https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide#page
		///
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		public static void TrackSuccessfulSignIn(HttpClient httpClient, string host, string trackingId, string googleClientId)
		{
			if (string.IsNullOrEmpty(trackingId))
				throw new ArgumentNullException(nameof(trackingId));

			if (string.IsNullOrEmpty(googleClientId))
			{
				//Do nothing. AD logins dont have a googleClientId, we are currently not tracking AD logins
			}
            else
            {
				Track(httpClient, trackingId, googleClientId, host, googleTypeEvent, "IDAM", "Sign-in", "Successful sign in", 1);
				Track(httpClient, trackingId, googleClientId, host, googleTypePageview, "IDAM", "Sign-in", "Successful sign in", 1);
			}
		}

		public static async Task<HttpResponseMessage> Track(HttpClient httpClient, string trackingId, string googleClientId, string host, string type, string category, string action, string label, int? value = null)
		{
			if (string.IsNullOrEmpty(category))
				throw new ArgumentNullException(nameof(category));

			if (string.IsNullOrEmpty(action))
				throw new ArgumentNullException(nameof(action));

			var postData = new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("v", googleVersion),
				new KeyValuePair<string, string>("tid", trackingId),
				new KeyValuePair<string, string>("cid", googleClientId),
				new KeyValuePair<string, string>("t", type)
			};

			if (type == googleTypeEvent)
			{
				postData.Add(new KeyValuePair<string, string>("ec", category));
				postData.Add(new KeyValuePair<string, string>("ea", action));
			}
			else if (type == googleTypePageview)
			{
				postData.Add(new KeyValuePair<string, string>("dh", host));
				postData.Add(new KeyValuePair<string, string>("dp", "/virtual/sign-in-success"));
				postData.Add(new KeyValuePair<string, string>("dt", "Sign in success | IDAM"));
			}

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