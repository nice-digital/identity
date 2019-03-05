using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.APIModels.Responses
{
	public class UserInList
	{
		public UserInList(User user)
		{
			UserId = user.UserId;
			Auth0UserId = user.Auth0UserId;
			FirstName = user.FirstName;
			LastName = user.LastName;
			Email = user.EmailAddress;
		}

		public int UserId { get; }
		public string Auth0UserId { get; }
		public string FirstName { get; }
		public string LastName { get; }
		public string Email { get; }
	}
}
