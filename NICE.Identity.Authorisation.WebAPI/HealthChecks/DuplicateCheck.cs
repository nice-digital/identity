using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public class DuplicateCheck : IHealthCheck
	{
		private readonly IdentityContext dbContext;

		public DuplicateCheck(IdentityContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			var duplicateEmails = dbContext.Users
				.GroupBy(u => u.EmailAddress)
				.Where(u => u.Count() > 1)
				.Select(u => u.Key)
				.ToList();


			if (duplicateEmails.Count == 0)
				return Task.FromResult(HealthCheckResult.Healthy());


			return Task.FromResult(HealthCheckResult.Unhealthy(description: $"Duplicate email addresses: {string.Join(',', duplicateEmails)}"));
		}
	}
}
