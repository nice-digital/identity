using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.APIModels.Responses
{
	public class UserInList
	{
		public UserInList(User user)
		{
			
			Auth0UserId = user.Auth0UserId;
			FirstName = user.FirstName;
			LastName = user.LastName;
		}

		public string Auth0UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
