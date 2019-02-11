namespace NICE.Identity.Authorisation.WebAPI.Models.Responses
{
	public enum ClaimType
	{
		Role,
		FirstName
	}

    public class Claim
    {
	    public Claim(ClaimType type, string value)
	    {
		    Type = type;
		    Value = value;
	    }

	    public ClaimType Type { get; set; }
        public string Value { get; set; }
    }
}