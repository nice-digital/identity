using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NICE.Identity.Authentication.Sdk.Abstractions;

namespace NICE.Identity.TestClient.NETCore.Authorisation
{
	public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
	{
		private readonly IAuthorisationService _authorisationService;

		public RoleRequirementHandler(IAuthorisationService authorisationService)
		{
			_authorisationService = authorisationService ?? throw new ArgumentNullException(nameof(authorisationService));
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
		{
			if (!context.User.HasClaim(x => x.Type == "userId"))
			{
				
			}
		}
	}
}