namespace NICE.Identity.NETFramework.Nuget
{
	public class AuthConfiguration
	{
		public string Domain { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string RedirectUri { get; set; }
		public string PostLogoutRedirectUri { get; set; }
	}
}
