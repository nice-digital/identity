using Microsoft.AspNetCore.Authorization;

namespace NICE.Identity.Authentication.Sdk.Authorisation
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