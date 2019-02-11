using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Domain;

namespace NICE.Identity.Authorisation.WebAPI.Abstractions
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetByUserId(string userId);

        Task AddToUser(Role role);
    }
}