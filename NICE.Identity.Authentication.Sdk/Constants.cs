namespace NICE.Identity.Authentication.Sdk
{
	public static class Constants
	{
		public static class AuthorisationURLs
		{
			public const string GetClaims = "/api/claims/";

			private const string ApiUsersPath = "/api/users/";

			public const string FindUsersRoute = "findusers";
			public const string FindUsersFullPath = ApiUsersPath + FindUsersRoute;

			public const string FindRolesRoute = "findroles/";
			public const string FindRolesFullPath = ApiUsersPath + FindRolesRoute;
			
			public const string GetOrganisationsRoute = "getorganisations";
			public const string GetOrganisationsFullPath = "/api/organisations/" + GetOrganisationsRoute;
		}

		public static class IdTokenPayload
        {
			public const string tempCid = "http://nice.org.uk/tempCid";
		}
	}
}
