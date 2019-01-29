using System.Text.RegularExpressions;

namespace NICE.Identity.Test.Infrastructure
{
	/// <summary>
	/// These Scrubbers, scrub out text in integration test output so that the integration test is rerunnable.
	/// eg, removing a date or time in the page.
	/// </summary>
    public static class Scrubbers 
    {
       /// <summary>
		/// Intended to scrub out the data-cookie-string attribute in some html, a bit like this:
		/// &lt;button type="button" class="btn btn-default navbar-btn" data-cookie-string=".AspNet.Consent=yes; expires=Thu, 02 Jan 2020 14:29:04 GMT; path=/; samesite=lax" /&gt;	
		/// 
		/// unescaped regex: data-cookie-string=\"([^\"]+)\"
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ScrubCookieString(string str)
	    {
			return Regex.Replace(str, @"data-cookie-string=\""([^\""]+)\""", @"data-cookie-string\""scrubbed by ScrubCookieString""");
	    }

		/// <summary>
		/// Scrubs out the auditId in some json.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
	    public static string ScrubAuditId(string str)
	    {
			return Regex.Replace(str, @"""auditId"":(\d+)", @"""auditId"":""scrubbed by ScrubAuditId""");
		}

		public static string ScrubHashFromJavascriptFileName(string str)
		{
			return Regex.Replace(str, "(src=\\\".*.)(.[a-z0-9]{8}.)(js\\\")", "$1.$3"); //unescaped regex is: src=\".*.([a-z0-9]{8}.)js
		}
		public static string ScrubVersion(string str)
		{
			return Regex.Replace(str, @"\?v=([^\""]+)", "?v=versionScrubbedByScrubVersion"); //unescaped regex is: \?v=([^\"]+)
		}
		public static string ScrubErrorMessage(string str)
		{
		 return Regex.Replace(str, "(<!--)([\\d\\D]+)(-->)", "\"Error Message\":\"scrubbed by ScrubErrorMessage\"");

		}
	}
}
