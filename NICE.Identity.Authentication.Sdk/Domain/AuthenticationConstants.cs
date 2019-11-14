namespace NICE.Identity.Authentication.Sdk.Domain
{
	public static class AuthenticationConstants
	{
		public static readonly string AuthenticationScheme = "Auth0";

		/// <summary>
		/// This issuer string is used for claims like the user's name, which is owned by IdAMIssuer.
		/// Other issuers are typically going to be the website host for roles.
		/// </summary>
		public static readonly string IdAMIssuer = "IdAMIssuer";

		public static readonly string CookieName = "IdAM";
	}
}