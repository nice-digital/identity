using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
    public partial class UserRoles
    {
        public int UserRoleId { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public Roles Role { get; set; }
        public Users User { get; set; }
    }
}
