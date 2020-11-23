using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public interface IUserSyncCheck : IHealthCheck
	{
		Task<UserSync> GetUserSyncData();
	}

	public class UserSyncCheck : IUserSyncCheck
	{
		private readonly IProviderManagementService _providerManagementService;
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public UserSyncCheck(IProviderManagementService providerManagementService, IServiceScopeFactory serviceScopeFactory)
		{
			_providerManagementService = providerManagementService;
			_serviceScopeFactory = serviceScopeFactory;
		}

		public async Task<UserSync> GetUserSyncData()
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetService<IdentityContext>(); //dbContext not thread safe, so creating in it's own scope.

				var userCountLocalDB = dbContext.Users.ToList();
				var userCountLocalDbWhereMarkedInRemote = userCountLocalDB.Count(u => u.IsInAuthenticationProvider);

				var lastTenUsersAndTotalCountFromProvider = await _providerManagementService.GetLastTenUsersAndTotalCount();

				var usersNotInLocalDB = lastTenUsersAndTotalCountFromProvider.last10Users
					.Where(auth0User => !userCountLocalDB.Any(localUser =>
						localUser.NameIdentifier.Equals(auth0User.NameIdentifier, StringComparison.OrdinalIgnoreCase)))
					.ToList();

				return new UserSync(userCountLocalDB: userCountLocalDB.Count,
					userCountLocalDbWhereMarkedInRemote: userCountLocalDbWhereMarkedInRemote,
					userCountRemoteDB: lastTenUsersAndTotalCountFromProvider.totalUsersCount,
					usersNotInLocalDbOfTheTenMostRecent: usersNotInLocalDB);
			}
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			var userSyncData = await GetUserSyncData();

			var countsMatch = userSyncData.UserCountLocalDBWhereMarkedInRemote.Equals(userSyncData.UserCountRemoteDB);

			var usersPresentInLocalDB = !userSyncData.UsersNotInLocalDBOfTheTenMostRecent.Any();
			
			if (countsMatch && usersPresentInLocalDB)
				return HealthCheckResult.Healthy();

			var usersNotInDBDescription = "Users out of sync.";

			if (countsMatch && !usersPresentInLocalDB)
				return HealthCheckResult.Unhealthy(description: $"The user counts match. {usersNotInDBDescription}"); //

			var countsNotMatchingErrorText = $"The user counts don't match.";

			if (usersPresentInLocalDB)
				return HealthCheckResult.Unhealthy(description: $"{countsNotMatchingErrorText} However the users are in sync.");

			return HealthCheckResult.Unhealthy(description: $"{countsNotMatchingErrorText} {usersNotInDBDescription}");
		}
	}
}