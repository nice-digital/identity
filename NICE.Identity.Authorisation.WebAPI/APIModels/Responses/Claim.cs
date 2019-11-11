using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Responses
{


	public class Claim
    {
	    public Claim(string type, string value, string issuer)
	    {
		    Type = type;
		    Value = value;
		    Issuer = issuer;
	    }

	    public string Type { get; set; }
        public string Value { get; set; }
		public string Issuer { get; set; }
	}
}