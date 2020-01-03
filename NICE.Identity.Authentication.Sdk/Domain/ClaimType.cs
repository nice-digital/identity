using System.Security.Claims;

namespace NICE.Identity.Authentication.Sdk.Domain
{
	public static class ClaimType
	{
		/// <summary>
		/// this id is the userId column in the database. it's only for use internally by IdAM. 
		/// </summary>
		public static readonly string IdAMId = "http://identity.nice.org.uk/claims/user-id";

		/// <summary>
		/// this name identifier was the Auth0UserId in the database, it's the id to be used by authenticating clients.
		/// </summary>
		public static readonly string NameIdentifier = "http://identity.nice.org.uk/claims/name-identifier";

		public static readonly string FirstName = "http://identity.nice.org.uk/claims/first-name";
		public static readonly string LastName = "http://identity.nice.org.uk/claims/last-name";
		public static readonly string Role = "http://identity.nice.org.uk/claims/role";
		public static readonly string TermsAndConditions = "http://identity.nice.org.uk/claims/terms-and-conditions";
		public static readonly string IsStaff = "http://identity.nice.org.uk/claims/is-staff";
		public static readonly string EmailAddress = "http://identity.nice.org.uk/claims/email-address";
		public static readonly string DisplayName = "http://identity.nice.org.uk/claims/display-name";
		public static readonly string IsMigrated = "http://identity.nice.org.uk/claims/is-migrated";
	}
}
