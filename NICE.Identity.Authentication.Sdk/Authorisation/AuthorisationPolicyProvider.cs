using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class AuthorisationPolicyProvider : DefaultAuthorizationPolicyProvider
	{
		private readonly AuthorizationOptions _options;
		private readonly IConfiguration _configuration;

		public AuthorisationPolicyProvider(IOptions<AuthorizationOptions> options, IConfiguration configuration) : base(options)
		{
			_options = options.Value;
			_configuration = configuration;
		}

		public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			// Check static policies first
			var policy = await base.GetPolicyAsync(policyName);

			if (policy == null)
			{
				policy = new AuthorizationPolicyBuilder()
					.AddRequirements(new RoleRequirement(policyName))
					.Build();

				// Add policy to the AuthorizationOptions, so we don't have to re-create it each time
				_options.AddPolicy(policyName, policy);
			}

			return policy;
		}
	}
}
