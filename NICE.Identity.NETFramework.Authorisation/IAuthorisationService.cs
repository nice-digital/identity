using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace NICE.Identity.NETFramework.Authorisation
{
    public interface IAuthorisationService
    {
        Task<IEnumerable<Claim>> GetByUserId(string userId);

        Task CreateOrUpdate(string userId, Claim role);

        Task<bool> UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roles);
    }
}