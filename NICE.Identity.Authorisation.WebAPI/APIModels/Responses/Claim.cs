using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Responses
{
	public static class ClaimTypesIdAM
	{
		public const string TermsAndConditions = "TermsAndConditions";
	}

	public static class ClaimConstants {
		/// <summary>
		/// This issuer string is used for claims like the user's name, which is owned by IdAM.
		/// Other issuers are typically going to be the website host for roles.
		/// </summary>
		public const string IdAMIssuer = "IdAM";
	}

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