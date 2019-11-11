namespace NICE.Identity.Authentication.Sdk.Domain
{
	public static class Issuers
	{
		/// <summary>
		/// This issuer string is used for claims like the user's name, which is owned by IdAM.
		/// Other issuers are typically going to be the website host for roles.
		/// </summary>
		public static readonly string IdAM = "IdAM";
	}
}