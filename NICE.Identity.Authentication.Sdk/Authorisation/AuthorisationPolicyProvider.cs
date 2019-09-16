using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NICE.Identity.Authentication.Sdk.Authentication;
using NICE.Identity.Authentication.Sdk.Configuration;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class AuthorisationPolicyProvider : DefaultAuthorizationPolicyProvider
	{
		private readonly AuthorizationOptions _options;
		//private readonly IAuth0Configuration _configuration;
		private readonly string _domain;
		public AuthorisationPolicyProvider(IOptions<AuthorizationOptions> options, IAuthConfiguration authConfiguration) : base(options)
		{
			_options = options.Value;
			_domain = $"https://{authConfiguration.TenantDomain}/";
		}

		public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			// Check static policies first
			var policy = await base.GetPolicyAsync(policyName);

			if (policy == null)
			{
				if (policyName.Contains(":"))
				{
					policy = new AuthorizationPolicyBuilder()
						.AddRequirements(new ScopeRequirement(policyName, _domain))
						.Build();
				}
				else
				{
					policy = new AuthorizationPolicyBuilder()
						.AddRequirements(new RoleRequirement(policyName))
						.Build();
				}

				// Add policy to the AuthorizationOptions, so we don't have to re-create it each time
				_options.AddPolicy(policyName, policy);
			}

			return policy;
		}
	}
}
