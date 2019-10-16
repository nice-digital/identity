using Newtonsoft.Json;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Requests
{
    public class VerificationEmail
    {
	    [JsonProperty("user_id")]
		public string UserId { get; set; }
    }
}