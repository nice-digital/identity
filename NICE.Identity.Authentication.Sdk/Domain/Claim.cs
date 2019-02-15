namespace NICE.Identity.Authentication.Sdk.Domain
{
    public class Claim
    {
        public string Value { get; set; }

        public string Type { get; set; }

        public Claim(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}