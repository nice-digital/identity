using System.Collections.Generic;
using NICE.Identity.Authorisation.WebAPI.ApiModels;
using User = NICE.Identity.Authorisation.WebAPI.ApiModels.User;

namespace NICE.Identity.Authorisation.WebAPI.APIModels
{
    public class UserAndRoleByWebsite
    {
        public UserAndRoleByWebsite()
        {
        }

        public int WebsiteId { get; set; }
        public int ServiceId { get; set; }
        public int EnvironmentId { get; set; }
        public List<UserAndRoles> UsersAndRoles { get; set; }
        public Service Service { get; set; }
        public Website Website { get; set; }
        public Environment Environment { get; set; }
        public List<Role> AllRoles { get; set; }
    }

    public class UserAndRoles
    {
        public UserAndRoles(int userId, User user, List<UserRoleDetailed> roles)
        {
            UserId = userId;
            User = user;
            Roles = roles;
        }

        public int UserId { get; set; }
        public User User { get; set; }
        public List<UserRoleDetailed> Roles { get; set; }
    }
}
