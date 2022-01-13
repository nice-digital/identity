using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{

    public class User
	{
        public User()
        {
        }
        public User(DataModels.User user)
        {
            UserId = user.UserId;
            NameIdentifier = user.NameIdentifier;
            FirstName = user.FirstName;
            LastName = user.LastName;
            AllowContactMe = user.AllowContactMe; 
            InitialRegistrationDate = user.InitialRegistrationDate;
            LastLoggedInDate = user.LastLoggedInDate;
            HasVerifiedEmailAddress = user.HasVerifiedEmailAddress;
            EmailAddress = user.EmailAddress;
            IsLockedOut = user.IsLockedOut;
            IsStaffMember = user.IsStaffMember;
            IsMigrated = user.IsMigrated;
            IsInAuthenticationProvider = user.IsInAuthenticationProvider;
            UserEmailHistory = user.UserEmailHistory;

            if (user.UserRoles != null)
            {
	            var websitesTheUserHasAccessTo = new List<int>();

                var roles = user.UserRoles.Select(ur => ur.Role).ToList();
                websitesTheUserHasAccessTo.AddRange(roles.Select(role => role.WebsiteId).Distinct());

                HasAccessToWebsiteIds = websitesTheUserHasAccessTo;
            }
        }

        public int? UserId { get; set; }
		public string NameIdentifier { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public bool? AllowContactMe { get; set; }
		public bool? HasVerifiedEmailAddress { get; set; }
		public bool? IsLockedOut { get; set; }
		public DateTime? InitialRegistrationDate { get; set; }
        public DateTime? LastLoggedInDate { get; set; }
		public bool? IsStaffMember { get; set; }
		public bool? AcceptedTerms { get; set; }
		public bool? IsMigrated { get; set; }
		public bool? IsInAuthenticationProvider { get; set; }

		public IEnumerable<int> HasAccessToWebsiteIds { get; set; } = new List<int>();

		public IEnumerable<DataModels.UserEmailHistory> UserEmailHistory { get; set; }
    }
}