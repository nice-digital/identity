using System;
using Newtonsoft.Json;

namespace NICE.Identity.Authentication.Sdk.Abstractions
{
	public class JwtToken
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("expires_in")]
		public string ExpiresIn { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }
	}
}