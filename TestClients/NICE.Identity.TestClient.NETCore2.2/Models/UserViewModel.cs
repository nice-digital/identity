using System;

namespace NICE.Identity.TestClient.NetCore.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
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
    }
}