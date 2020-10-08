namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	/// <summary>
	/// This class is used by the HealthCheck.DuplicateCheck code, to return duplicate users to the admin api.
	/// </summary>
	public class BasicUserInfo
	{
		public BasicUserInfo(string nameIdentifier, string emailAddress)
		{
			NameIdentifier = nameIdentifier;
			EmailAddress = emailAddress;
		}

		public string NameIdentifier { get; private set; }
		public string EmailAddress { get; private set; }
	}
}