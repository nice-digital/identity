using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.ApiModels;

namespace NICE.Identity.Authorisation.WebAPI.APIModels
{
	public class UserWithRoles : User
	{
		public ICollection<UserRole> UserRoles { get; set; }

		public UserWithRoles(DataModels.User user) : base(user)
		{
			UserRoles = user.UserRoles.Select(userRole => new UserRole(userRole)).ToList();
		}
	}
}
