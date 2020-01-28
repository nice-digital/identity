namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	/// <summary>
	/// Policies are dynamic, they can be added, removed or renamed in the database at any time.
	/// 
	/// The policies here are therefore non-exhaustive, and should only contain ones that we expect to never change.
	/// </summary>
    public static class Policies
    {

		//web policies have been removed. see class comment.


		/// <summary>
		/// By convention API policy names (scopes) have a colon in them. 
		/// </summary>
		public static class API
		{
			/// <summary>
			/// This role has admin capabilities for users.
			/// </summary>
			public const string UserAdministration = "User:Administration";

			/// <summary>
			/// This following string is used in the API to secure the FindUsers and FindRoles actions. The actual roles with access to these
			/// functions are defined in configuration and replaced in, in the RoleRequirementHandler.
			/// </summary>
		    public const string RolesWithAccessToUserProfilesPlaceholder = "Placeholder:Roles_are_replaced_here_at_runtime_from_config";

			
	    }
    }
}