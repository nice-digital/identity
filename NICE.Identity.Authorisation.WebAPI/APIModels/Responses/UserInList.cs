using System;
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
			AllowContactMe = user.AllowContactMe;
			HasVerifiedEmailAddress = user.HasVerifiedEmailAddress;
			IsLockedOut = user.IsLockedOut;
			InitialRegistrationDate = user.InitialRegistrationDate;

		}
		public int UserId { get; }
		public string Auth0UserId { get; }
		public string FirstName { get; }
		public string LastName { get; }
		public string Email { get; }
		public bool AllowContactMe { get; set; }
		public bool HasVerifiedEmailAddress { get; set; }
		public bool IsLockedOut { get; set; }
		public DateTime? InitialRegistrationDate { get; set; }
	}
}