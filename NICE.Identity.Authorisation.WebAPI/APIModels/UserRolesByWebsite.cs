using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class UserRolesByWebsite
    {
        public int UserId;
        public int WebsiteId;
        public int ServiceId;
        public List<UserRoleDetailed> Roles;
        public Service Service;
        public Website Website;
    }
}