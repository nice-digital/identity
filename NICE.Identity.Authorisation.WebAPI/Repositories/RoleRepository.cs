using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Abstractions;
using NICE.Identity.Authorisation.WebAPI.Domain;

namespace NICE.Identity.Authorisation.WebAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public async Task<IEnumerable<Role>> GetByUserId(string userId)
        {
            var roles = new List<Role>();

            // TODO Entity frameowrk code

            return roles;
        }

        public async Task AddToUser(Role role)
        {
            // TODO Entity framework code
        }
    }
}