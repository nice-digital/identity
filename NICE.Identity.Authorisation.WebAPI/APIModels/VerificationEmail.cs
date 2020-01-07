using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class VerificationEmail
    {
	    [JsonPropertyName("user_id")]
		public string UserId { get; set; }
    }
}