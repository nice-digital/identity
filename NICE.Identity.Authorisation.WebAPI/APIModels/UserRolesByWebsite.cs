using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class UserRolesByWebsite
    {
        public int UserId;
        public int WebsiteId;
        public int ServiceId;
        public List<WebsiteRole> Roles;
        public Service Service;
        public Website Website;
    }

    public class WebsiteRole
    {
        public int Id;
        public string Name;
        public string Description;
        public bool HasRole;
    }
}