using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.NETFramework.Nuget.Domain;

namespace NICE.Identity.NETFramework.Nuget.Abstractions
{
    public interface IAuthorisationService
    {
        Task<IEnumerable<Claim>> GetByUserId(string userId);

        Task CreateOrUpdate(string userId, Claim role);

	    Task<bool> UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roles);
    }
}