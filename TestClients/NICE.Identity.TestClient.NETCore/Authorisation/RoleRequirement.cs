using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace NICE.Identity.TestClient.NETCore.Authorisation
{
	public class RoleRequirement : IAuthorizationRequirement
	{
		//public IEnumerable<string> RoleName { get; set; }

		public string Role { get; set; }

		public RoleRequirement(string role)
		{
			Role = role;
		}
	}
}