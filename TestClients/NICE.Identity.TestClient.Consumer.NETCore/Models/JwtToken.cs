using Newtonsoft.Json;

namespace NICE.Identity.TestClient.M2MApp.Models
{
    public class JwtToken
    {
	    [JsonProperty("access_token")]
		public string AccessToken { get; set; }

	    [JsonProperty("token_type")]
		public string TokenType { get; set; }

	    [JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }
	}
}
