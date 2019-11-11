namespace NICE.Identity.Authentication.Sdk.Domain
{
	public static class ClaimType
	{
		/// <summary>
		/// this id is the userId column in the database. it's only for use internally by IdAM. 
		/// </summary>
		public static readonly string IdAMId = "user-id";

		/// <summary>
		/// this name identifier is the Auth0UserId in the database, it's the id to be used by authenticating clients.
		/// </summary>
		public static readonly string NameIdentifier = "name-identifier";

		public static readonly string FirstName = "first-name";
		public static readonly string LastName = "last-name";
		public static readonly string Role = "role";
		public static readonly string TermsAndConditions = "terms-and-conditions";
		public static readonly string IsStaff = "is-staff";
		public static readonly string EmailAddress = "email-address";
	}
}
