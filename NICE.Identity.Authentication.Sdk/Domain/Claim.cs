namespace NICE.Identity.Authentication.Sdk.Domain
{
	public class Claim
    {
        public string Value { get; set; }

		//todo: make type a ClaimType, not a string.
        public string Type { get; set; }

        public string Issuer { get; set; }

		public Claim(string type, string value, string issuer)
        {
            Type = type;
            Value = value;
            Issuer = issuer;
        }
    }
}