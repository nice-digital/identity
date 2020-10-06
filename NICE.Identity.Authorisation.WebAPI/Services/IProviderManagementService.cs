using System.Threading.Tasks;
using Auth0.Core.Collections;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
    public interface IProviderManagementService
    {
        Task UpdateUser(string authenticationProviderUserId, User user);

        Task DeleteUser(string authenticationProviderUserId);

        Task<string> GetAccessTokenForManagementAPI();

        Task RevokeRefreshTokensForUser(string nameIdentifier);

        Task<IPagedList<Auth0.ManagementApi.Models.User>> GetUsers();

    }
}