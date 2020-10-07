using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public class UserSyncCheck : IHealthCheck
	{
		private readonly IProviderManagementService _providerManagementService;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UserSyncCheck(IProviderManagementService providerManagementService, IServiceScopeFactory serviceScopeFactory)
		{
			_providerManagementService = providerManagementService;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetService<IdentityContext>(); //dbContext not thread safe, so creating in it's own scope.

				var allUsersInLocalDB = dbContext.Users.ToList();

				var lastTenUsersAndTotalCountFromProvider =
					await _providerManagementService.GetLastTenUsersAndTotalCount();


				var countsMatch = allUsersInLocalDB.Count.Equals(lastTenUsersAndTotalCountFromProvider.totalUsersCount);

				var usersNotInLocalDB = lastTenUsersAndTotalCountFromProvider.last10Users
					.Where(auth0User => !allUsersInLocalDB.Any(localUser =>
						localUser.NameIdentifier.Equals(auth0User.NameIdentifier, StringComparison.OrdinalIgnoreCase)))
					.ToList();

				var usersPresentInLocalDB = !usersNotInLocalDB.Any();
				var usersNotInDBDescription = usersPresentInLocalDB
					? null
					: $"The following of the 10 most recent users in the provider database are not in the local database: {string.Join(',', usersNotInLocalDB.Select(u => u.NameIdentifier))}";

				if (countsMatch && usersPresentInLocalDB)
					return HealthCheckResult.Healthy();

				if (countsMatch && !usersPresentInLocalDB)
					return HealthCheckResult.Unhealthy(description: $"The user counts match ({allUsersInLocalDB.Count}). {usersNotInDBDescription}"); //

				var countsNotMatchingErrorText = $"The user counts don't match between the provider user table ({lastTenUsersAndTotalCountFromProvider.totalUsersCount}) and the local database ({allUsersInLocalDB.Count})."; //({lastTenUsersAndTotalCountFromProvider.totalUsersCount}) //({allUsersInLocalDB.Count})

				if (usersPresentInLocalDB)
					return HealthCheckResult.Unhealthy(description: $"{countsNotMatchingErrorText} However the most recent 10 users in the provider are in the local database.");

				return HealthCheckResult.Unhealthy(description: $"{countsNotMatchingErrorText} {usersNotInDBDescription}");
			}
		}
	}
}
