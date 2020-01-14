using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class UserRolesByWebsite
    {
        public int UserId { get; set; }
        public int WebsiteId { get; set; }
        public int ServiceId { get; set; }
        public List<UserRoleDetailed> Roles { get; set; }
        public Service Service { get; set; }
        public Website Website { get; set; }
    }
}