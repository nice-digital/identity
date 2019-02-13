using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.Abstractions
{
    public interface IAuthorisationService
    {
        Task<IEnumerable<Claim>> GetByUserId(string userId);

        Task CreateOrUpdate(string userId, Claim role);

	    Task<bool> UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roleName);
    }
}