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

        public User(ApiModels.User user)
        {
	        NameIdentifier = user.NameIdentifier;
	        FirstName = user.FirstName;
	        LastName = user.LastName;
	        AllowContactMe = user.AllowContactMe.HasValue && user.AllowContactMe.Value;
	        InitialRegistrationDate = user.InitialRegistrationDate;
	        LastLoggedInDate = user.LastLoggedInDate;
	        HasVerifiedEmailAddress = user.HasVerifiedEmailAddress.HasValue && user.HasVerifiedEmailAddress.Value;
	        EmailAddress = user.EmailAddress;
	        IsLockedOut = user.IsLockedOut.HasValue && user.IsLockedOut.Value;
	        IsStaffMember = user.IsStaffMember.HasValue && user.IsStaffMember.Value;
	        IsMigrated = user.IsMigrated.HasValue && user.IsMigrated.Value;
	        IsInAuthenticationProvider = user.IsInAuthenticationProvider.HasValue && user.IsInAuthenticationProvider.Value;
            IsMarkedForDeletion = user.IsMarkedForDeletion.HasValue && user.IsMarkedForDeletion.Value;
        }

        public User(int userId, string nameIdentifier, string firstName, string lastName, bool acceptedTerms, bool allowContactMe, DateTime? initialRegistrationDate, DateTime? lastLoggedInDate, bool hasVerifiedEmailAddress, string emailAddress, bool isLockedOut, bool isStaffMember, bool isMigrated, bool isInAuthenticationProvider, bool dormantAccountWarningSent)
        {
            UserId = userId;
            NameIdentifier = nameIdentifier;
            FirstName = firstName;
            LastName = lastName;
            AllowContactMe = allowContactMe;
            InitialRegistrationDate = initialRegistrationDate;
            LastLoggedInDate = lastLoggedInDate;
            HasVerifiedEmailAddress = hasVerifiedEmailAddress;
            EmailAddress = emailAddress;
            IsLockedOut = isLockedOut;
            IsStaffMember = isStaffMember;
            UserRoles = new HashSet<UserRole>();
            IsMigrated = isMigrated;
            IsInAuthenticationProvider = isInAuthenticationProvider;
            IsMarkedForDeletion = dormantAccountWarningSent;
        }

        public int UserId { get; set; }
        public string NameIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AllowContactMe { get; set; }
        public DateTime? InitialRegistrationDate { get; set; }
        public DateTime? LastLoggedInDate { get; set; }
        public bool HasVerifiedEmailAddress { get; set; }
        public string EmailAddress { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsStaffMember { get; set; }
        public bool IsMigrated { get; set; }
        public bool IsInAuthenticationProvider { get; set; }
        public bool IsMarkedForDeletion { get; set; }

		public ICollection<UserRole> UserRoles { get; set; }
		public ICollection<UserEmailHistory> UserEmailHistory { get; set; }
		public ICollection<UserEmailHistory> ArchivedUserEmailHistory { get; set; }
        public ICollection<TermsVersion> UserCreatedTermsVersions { get; set; }
        public ICollection<UserAcceptedTermsVersion> UserAcceptedTermsVersions { get; set; }
        public ICollection<Job> Jobs { get; set; }

        public string DisplayName => $"{FirstName} {LastName}".Trim();

		public UserAcceptedTermsVersion LatestAcceptedTermsVersion()
        {
            return (UserAcceptedTermsVersions ?? new List<UserAcceptedTermsVersion>()).ToList().OrderByDescending(x => x.TermsVersionId).FirstOrDefault();
        }

		/// <summary>
		/// This method is only called by the UpdateUser action. the API model user coming in is only partially set by the front-end, as each component just hits the updateuser
		/// endpoint with it's own data, not with a complete user object.
		/// </summary>
		/// <param name="user"></param>
        public void UpdateFromApiModel(ApiModels.User user)
        {
			NameIdentifier = user.NameIdentifier ?? NameIdentifier;
			FirstName = user.FirstName ?? FirstName;
			LastName = user.LastName ?? LastName;
			AllowContactMe = user.AllowContactMe ?? AllowContactMe;
			InitialRegistrationDate = user.InitialRegistrationDate ?? InitialRegistrationDate;
			LastLoggedInDate = user.LastLoggedInDate ?? LastLoggedInDate;
			HasVerifiedEmailAddress = user.HasVerifiedEmailAddress ?? HasVerifiedEmailAddress;
			EmailAddress = user.EmailAddress ?? EmailAddress;
			IsLockedOut = user.IsLockedOut ?? IsLockedOut;
			IsStaffMember = user.IsStaffMember ?? IsStaffMember;
			IsMigrated = user.IsMigrated ?? IsMigrated;
			IsInAuthenticationProvider = user.IsInAuthenticationProvider ?? IsInAuthenticationProvider;
            IsMarkedForDeletion = user.IsMarkedForDeletion ?? IsMarkedForDeletion;
        }
    }
}
