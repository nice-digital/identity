using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public class DuplicateCheck : IHealthCheck
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DuplicateCheck(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetService<IdentityContext>(); //dbContext not thread safe, so creating in it's own scope.

				var duplicateEmails = dbContext.Users
					.GroupBy(u => u.EmailAddress)
					.Where(u => u.Count() > 1)
					.Select(u => u.Key)
					.ToList();


				if (duplicateEmails.Count == 0)
					return Task.FromResult(HealthCheckResult.Healthy());


				return Task.FromResult(
					HealthCheckResult.Unhealthy(
						description: $"Duplicate email addresses: {string.Join(',', duplicateEmails)}"));
			}
		}
	}
}
