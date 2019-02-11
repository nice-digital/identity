using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.Abstractions
{
    public interface IAuthorisationService
    {
        Task<IEnumerable<Role>> GetByUserId(string userId);

        Task AddToUser(Role role);
    }
}