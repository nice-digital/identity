using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class Role
    {

        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

		public Role(int roleId, int websiteId, string name)
		{
			RoleId = roleId;
			WebsiteId = websiteId;
			Name = name;
			UserRoles = new HashSet<UserRole>(); ;
		}

		public int RoleId { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }

        public Website Website { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
