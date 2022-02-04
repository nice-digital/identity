using System.Collections.Generic;
using System.Data.SqlTypes;
using Auth0.ManagementApi.Models;

namespace NICE.Identity.Authorisation.WebAPI.APIModels
{

    public class UserAndRoleByWebsite
    {
        public UserAndRoleByWebsite()
        {
        }

        public int? UserId { get; set; }
        public string NameIdentifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool IsStaffMember { get; set; }
        public string EmailAddress { get; set; }
        public List<NICE.Identity.Authorisation.WebAPI.ApiModels.Role> Roles { get; set; }
    }
}
