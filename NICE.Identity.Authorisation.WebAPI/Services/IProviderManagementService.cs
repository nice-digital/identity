using NICE.Identity.Authorisation.WebAPI.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IProviderManagementService
	{
		Task<string> GetAccessTokenForManagementAPI();

        Task UpdateUser(string authenticationProviderUserId, User user);

        Task DeleteUser(string authenticationProviderUserId);

        Task RevokeRefreshTokensForUser(string nameIdentifier);

        Task<(int totalUsersCount, List<BasicUserInfo> last10Users)> GetLastTenUsersAndTotalCount();

        Task<Auth0.ManagementApi.Models.Job> VerificationEmail(string authenticationProviderUserId);
    }
}