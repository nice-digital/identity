using NICE.Identity.Authorisation.WebAPI.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IProviderManagementService
    {
        Task UpdateUser(string authenticationProviderUserId, User user);

        Task DeleteUser(string authenticationProviderUserId);

        Task<string> GetAccessTokenForManagementAPI();

        Task RevokeRefreshTokensForUser(string nameIdentifier);

        Task<(int totalUsersCount, List<BasicUserInfo> last10Users)> GetLastTenUsersAndTotalCount();
    }
}