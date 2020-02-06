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

	        Role = userRole.Role != null ? new Role(userRole.Role) : null;
        }

        [JsonPropertyName("id")]
        public int? UserRoleId { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }

        public Role Role { get; set; }
    }
    
    public class UserRoleDetailed
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool HasRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}