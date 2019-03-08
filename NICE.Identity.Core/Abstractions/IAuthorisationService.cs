using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Core.Domain;

namespace NICE.Identity.Core.Abstractions
{
    public interface IAuthorisationService
    {
        Task<IEnumerable<Claim>> GetByUserId(string userId);

        Task CreateOrUpdate(string userId, Claim role);
		
	    bool UserSatisfiesAtLeastOneRole(string userId, IEnumerable<string> roles);
	    Task<bool> UserSatisfiesAtLeastOneRoleAsync(string userId, IEnumerable<string> roles);
	}
}