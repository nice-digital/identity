using System;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
	public class User
	{
        public User()
        {
        }

        public User(int userId, string auth0UserId, string firstName, string lastName, string email, bool allowContactMe, bool hasVerifiedEmailAddress, bool isLockedOut, DateTime? initialRegistrationDate, DateTime? lastLoggedInDate, bool isStaffMember, bool acceptedTerms)
        {
            UserId = userId;
            Auth0UserId = auth0UserId;
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
        }

        public User(DataModels.User user)
        {
            UserId = user.UserId;
            Auth0UserId = user.Auth0UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            AllowContactMe = user.AllowContactMe;
            InitialRegistrationDate = user.InitialRegistrationDate;
            LastLoggedInDate = user.LastLoggedInDate;
            HasVerifiedEmailAddress = user.HasVerifiedEmailAddress;
            EmailAddress = user.EmailAddress;
            IsLockedOut = user.IsLockedOut;
            IsStaffMember = user.IsStaffMember;
        }

        public int UserId { get; set; }
		public string Auth0UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public bool AllowContactMe { get; set; }
		public bool HasVerifiedEmailAddress { get; set; }
		public bool IsLockedOut { get; set; }
		public DateTime? InitialRegistrationDate { get; set; }
        public DateTime? LastLoggedInDate { get; set; }
        public bool IsStaffMember { get; set; }
        public bool AcceptedTerms { get; set; }
    }
}