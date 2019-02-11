using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
    public partial class Users
    {
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public int UserId { get; set; }
        public string Auth0UserId { get; set; }
        public Guid? NiceaccountsId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AcceptedTerms { get; set; }
        public bool AllowContactMe { get; set; }
        public DateTime? InitialRegistrationDate { get; set; }
        public DateTime? LastLoggedInDate { get; set; }
        public bool HasVerifiedEmailAddress { get; set; }
        public string EmailAddress { get; set; }
        public string NiceactiveDirectoryUsername { get; set; }
        public string DsactiveDirectoryUsername { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsStaffMember { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
