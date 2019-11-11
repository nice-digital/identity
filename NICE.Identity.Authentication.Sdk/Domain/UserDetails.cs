namespace NICE.Identity.Authentication.Sdk.Domain
{
	/// <summary>
	/// This is very basic user details model. It's currently only used by consultation comments, which has a requirement to lookup user's details (as it doesn't store them).
	/// </summary>
	public class UserDetails
	{
		public UserDetails(string nameIdentifier, string displayName, string emailAddress)
		{
			NameIdentifier = nameIdentifier;
			DisplayName = displayName;
			EmailAddress = emailAddress;
		}

		public string NameIdentifier { get; private set; }
		public string DisplayName { get; private set; }
		public string EmailAddress { get; private set; }
	}
}
