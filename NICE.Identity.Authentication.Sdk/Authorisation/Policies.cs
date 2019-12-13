namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	/// <summary>
	/// Policies are dynamic, they can be added, removed or renamed in the database at any time.
	/// 
	/// The policies here are therefore non-exhaustive, and should only contain ones that we expect to never change.
	/// </summary>
    public static class Policies
    {
	    public static class Web
	    {
		    public const string Administrator = "Administrator"; //todo: remove

			/// <summary>
			/// This policy is used by the .net core test client to test authorisation of a route with a specific role.
			/// </summary>
		    public const string Editor = "Editor";
	    }

		/// <summary>
		/// By convention API policy names (scopes) have a colon in them. 
		/// </summary>
		public static class API
	    {
		    public const string UserAdministration = "User:Administration";
		}


		/// <summary>
		/// Policies to secure IdAM specific functionality.
		/// </summary>
		public static class IdAMSpecific
		{
			public const string WebsitePrefix = "Website:";

			public const string AllRolesForWebsiteComments = WebsitePrefix + "Consultation Comments";
		}
    }
}