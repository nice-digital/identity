using Microsoft.Extensions.Diagnostics.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI.HealthChecks
{
	public class UserSyncCheck : IHealthCheck
	{
		private readonly IdentityContext dbContext;
		private readonly IProviderManagementService _providerManagementService;

		public UserSyncCheck(IdentityContext dbContext, IProviderManagementService providerManagementService)
		{
			this.dbContext = dbContext;
			_providerManagementService = providerManagementService;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			var allUsersInDatabase = dbContext.Users.ToList();


			var last10UsersInAuth0 = await _providerManagementService.GetUsers();


			var paging = last10UsersInAuth0.Paging;


			return HealthCheckResult.Healthy();


			//return HealthCheckResult.Unhealthy(description: $"Duplicate email addresses: {"ff"}");
		}
	}
}
