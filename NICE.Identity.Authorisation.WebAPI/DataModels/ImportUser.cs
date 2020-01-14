using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
	public class ImportRole
	{
		/// <summary>
		/// the role id doesn't need to be supplied in the json. it is expected to be looked up using the rolename and website host.
		/// if it is supplied though, it's used.
		/// </summary>
		public int? RoleId { get; set; }

		public string RoleName { get; set; }
		public string WebsiteHost { get; set; }
	}

	public class ImportUser
	{
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }

		public IList<ImportRole> Roles { get; set; }

		public string NameIdentifier {
			get
			{
				if (Guid.TryParse(UserId, out var userGuid))
				{
					return $"{AuthenticationConstants.NameIdentifierDefaultPrefix}{userGuid.ToString().ToLower()}";
				}
				return UserId;
			}
		}

		public User AsUser => new DataModels.User()
			{
				NameIdentifier = NameIdentifier,
				FirstName = FirstName,
				LastName = LastName,
				EmailAddress = EmailAddress,
				AllowContactMe = false,
				IsMigrated = true,
				HasVerifiedEmailAddress = true,
				IsLockedOut = false,
				IsStaffMember = EmailAddress.Contains("@nice.org.uk", StringComparison.OrdinalIgnoreCase),
				IsInAuthenticationProvider = false
		};
	}
}