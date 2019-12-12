using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class UserRole
    {
	    public UserRole()
	    {
	    }

	    public UserRole(int userRoleId, int roleId, int userId)
		{
			UserRoleId = userRoleId;
			RoleId = roleId;
			UserId = userId;
		}

		public int UserRoleId { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }

        public Role Role { get; set; }
        public User User { get; set; }

        public void UpdateFromApiModel(ApiModels.UserRole userRole)
        {
            RoleId = userRole?.RoleId ?? RoleId;
            UserId = userRole?.UserId ?? UserId;
        }
    }
}
