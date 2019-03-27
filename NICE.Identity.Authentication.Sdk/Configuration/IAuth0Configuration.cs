namespace NICE.Identity.Authentication.Sdk.Configuration
{
	public interface IAuth0Configuration
	{
		string Domain { get; set; }
		string ClientId { get; set; }
		string GrantType { get; set; }
		string ClientSecret { get; set; }
		string ApiIdentifier { get; set; }
	}
}