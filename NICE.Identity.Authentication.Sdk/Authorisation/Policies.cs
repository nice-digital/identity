namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public static class Policies
    {
	    public static class Web
	    {
		    public const string Administrator = "Administrator";

		    public const string Editor = "Editor";
	    }

		/// <summary>
		/// By convention API policy names (scopes) have a colon in them. 
		/// </summary>
		public static class API
	    {
		    public const string UserAdmin = "User:Administration";
		}
    }
}