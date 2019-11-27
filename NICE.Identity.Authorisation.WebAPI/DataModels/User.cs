using System;
using System.Collections.Generic;
using System.Linq;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public User(int userId, string auth0UserId, string firstName, string lastName, bool acceptedTerms, bool allowContactMe, DateTime? initialRegistrationDate, DateTime? lastLoggedInDate, bool hasVerifiedEmailAddress, string emailAddress, bool isLockedOut, bool isStaffMember, bool isMigrated)
        {
            UserId = userId;
            Auth0UserId = auth0UserId;
            FirstName = firstName;
            LastName = lastName;
            //AcceptedTerms = acceptedTerms;
            AllowContactMe = allowContactMe;
            InitialRegistrationDate = initialRegistrationDate;
            LastLoggedInDate = lastLoggedInDate;
            HasVerifiedEmailAddress = hasVerifiedEmailAddress;
            EmailAddress = emailAddress;
            IsLockedOut = isLockedOut;
            IsStaffMember = isStaffMember;
            UserRoles = new HashSet<UserRole>();
            IsMigrated = isMigrated;
        }

        public int UserId { get; set; }
        public string Auth0UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? AllowContactMe { get; set; }
        public DateTime? InitialRegistrationDate { get; set; }
        public DateTime? LastLoggedInDate { get; set; }
        public bool? HasVerifiedEmailAddress { get; set; }
        public string EmailAddress { get; set; }
        public bool? IsLockedOut { get; set; }
        public bool? IsStaffMember { get; set; }
        public bool? IsMigrated { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<TermsVersion> UserCreatedTermsVersions { get; set; }
        public ICollection<UserAcceptedTermsVersion> UserAcceptedTermsVersions { get; set; }

        public UserAcceptedTermsVersion LatestAcceptedTermsVersion()
        {
            return (UserAcceptedTermsVersions ?? new List<UserAcceptedTermsVersion>()).ToList().OrderByDescending(x => x.TermsVersionId).FirstOrDefault();
        }

        public void UpdateFromApiModel(ApiModels.User user)
        {
            Auth0UserId = user?.Auth0UserId ?? Auth0UserId;
            FirstName = user?.FirstName ?? FirstName;
            LastName = user?.LastName ?? LastName;
            AllowContactMe = user?.AllowContactMe ?? AllowContactMe;
            InitialRegistrationDate = user?.InitialRegistrationDate ?? InitialRegistrationDate;
            LastLoggedInDate = user?.LastLoggedInDate ?? LastLoggedInDate;
            HasVerifiedEmailAddress = user?.HasVerifiedEmailAddress ?? HasVerifiedEmailAddress;
            EmailAddress = user?.EmailAddress ?? EmailAddress;
            IsLockedOut = user?.IsLockedOut ?? IsLockedOut;
            IsStaffMember = user?.IsStaffMember ?? IsStaffMember;
            IsMigrated = user?.IsMigrated ?? IsMigrated;
        }
    }
}
