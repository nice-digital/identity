using NICE.Identity.Authentication.Sdk.Domain;
using System;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	public class ImportUser
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }


		public string NameIdentifier =>
			$"{AuthenticationConstants.NameIdentifierDefaultPrefix}{UserId.ToString().ToLower()}";

		public User AsUser => new DataModels.User()
			{
				Auth0UserId = NameIdentifier,
				FirstName = FirstName,
				LastName = LastName,
				EmailAddress = EmailAddress,
				AllowContactMe = false,
				IsMigrated = true,
				HasVerifiedEmailAddress = true,
				IsLockedOut = false,
				IsStaffMember = EmailAddress.Contains("@nice.org.uk", StringComparison.OrdinalIgnoreCase)
			};
	}
}