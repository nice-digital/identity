namespace NICE.Identity.Authentication.Sdk.Abstractions
{
    public class AuthenticationServiceConfiguration
    {
        string Domain { get; set; }

        string ClientId { get; set; }

        string ClientSecret { get; set; }
    }
}