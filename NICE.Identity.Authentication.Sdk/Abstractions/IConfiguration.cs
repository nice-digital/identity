namespace NICE.Identity.Authentication.Sdk.Abstractions
{
    public interface IConfiguration
    {
        string Domain { get; set; }

        string ClientId { get; set; }

        string ClientSecret { get; set; }
    }
}