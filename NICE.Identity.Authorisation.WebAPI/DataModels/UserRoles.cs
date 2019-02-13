using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class UserRoles
    {
	    public UserRoles()
	    {
	    }

	    public UserRoles(int userRoleId, int roleId, int userId)
		{
			UserRoleId = userRoleId;
			RoleId = roleId;
			UserId = userId;
		}

		public int UserRoleId { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public Roles Role { get; set; }
        public Users User { get; set; }
    }
}
