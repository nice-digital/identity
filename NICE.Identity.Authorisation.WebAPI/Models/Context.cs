using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
	public partial class IdentityContext : DbContext
	{
		public List<UserRoles> GetClaims(int userId)
		{
			return UserRoles.Where(userRole => userRole.UserId.Equals(userId))
					.Include(userRoles => userRoles.Role)
					.Include(userRoles => userRoles.User)
					.ToList();
			
		}
	}
}
