using Newtonsoft.Json;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class UserRole
    {
        public UserRole()
        {
        }

        public UserRole(DataModels.UserRole userRole)
        {
            UserRoleId = userRole.UserRoleId;
            RoleId = userRole.RoleId;
            UserId = userRole.UserId;
        }

        [JsonProperty(PropertyName = "id")]
        public int? UserRoleId { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }
    }
}