using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
	public class User
	{
        public User()
        {
        }

        public User(int userId, string nameIdentifier, string firstName, string lastName, string email, bool allowContactMe, bool hasVerifiedEmailAddress, bool isLockedOut, DateTime? initialRegistrationDate, DateTime? lastLoggedInDate, bool isStaffMember, bool acceptedTerms, bool isMigrated, bool isInAuthenticationProvider, IEnumerable<int> hasAccessToServiceIds)
        {
            UserId = userId;
            NameIdentifier = nameIdentifier;
            FirstName = firstName;
            LastName = lastName;
            AllowContactMe = allowContactMe;
            InitialRegistrationDate = initialRegistrationDate;
            LastLoggedInDate = lastLoggedInDate;
            HasVerifiedEmailAddress = hasVerifiedEmailAddress;
            EmailAddress = email;
            IsLockedOut = isLockedOut;
            IsStaffMember = isStaffMember;
            AcceptedTerms = acceptedTerms;
            IsMigrated = isMigrated;
            IsInAuthenticationProvider = isInAuthenticationProvider;
            HasAccessToServiceIds = hasAccessToServiceIds;
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

            if (user.UserRoles != null)
            {
	            var servicesTheUserHasAccessTo = new List<int>();

                var roles = user.UserRoles.Select(ur => ur.Role).ToList();
                servicesTheUserHasAccessTo.AddRange(roles.Select(role => role.Website.ServiceId).Distinct());

                HasAccessToServiceIds = servicesTheUserHasAccessTo;
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

		public IEnumerable<int> HasAccessToServiceIds { get; set; } = new List<int>();
    }
}