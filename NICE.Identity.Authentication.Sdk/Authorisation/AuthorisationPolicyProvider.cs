#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NICE.Identity.Authentication.Sdk.Configuration;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class AuthorisationPolicyProvider : DefaultAuthorizationPolicyProvider
	{
		private readonly AuthorizationOptions _options;
		public AuthorisationPolicyProvider(IOptions<AuthorizationOptions> options, IAuthConfiguration authConfiguration) : base(options)
		{
			_options = options.Value;
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
#endif