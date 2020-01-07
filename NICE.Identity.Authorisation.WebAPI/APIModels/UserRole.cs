using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("id")]
        public int? UserRoleId { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }
    }
    
    public class UserRoleDetailed
    {
        [Required]
        public int Id;
        [Required]
        public bool HasRole;
        public string Name;
        public string Description;
    }
}