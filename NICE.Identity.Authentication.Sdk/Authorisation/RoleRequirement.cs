#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class RoleRequirement : IAuthorizationRequirement
	{
		public string Role { get; set; }

		public RoleRequirement(string role)
		{
			Role = role;
		}
	}
}
#endif