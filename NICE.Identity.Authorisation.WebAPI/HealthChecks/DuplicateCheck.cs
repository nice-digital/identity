using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public interface IDuplicateCheck : IHealthCheck
	{
		public IEnumerable<string> GetDuplicateUsers();
	}

	public class DuplicateCheck : IDuplicateCheck
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public DuplicateCheck(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		public IEnumerable<string> GetDuplicateUsers()
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetService<IdentityContext>(); //dbContext not thread safe when being called from a health check, so creating in it's own scope.

				var duplicateUsers = dbContext.Users
					.GroupBy(u => u.EmailAddress)
					.Where(u => u.Count() > 1)
					.Select(u => u.Key)
					.ToList();

				return duplicateUsers;
			}
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			var duplicateUsers = GetDuplicateUsers().ToList();

			if (duplicateUsers.Count == 0)
				return Task.FromResult(HealthCheckResult.Healthy());

			return Task.FromResult(HealthCheckResult.Unhealthy(description: $"Duplicate email addresses in database"));
		}
	}
}