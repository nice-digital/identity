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
        public UserRoleDetailed()
        {
        }

        public UserRoleDetailed(int id, bool hasRole, string name, string description)
        {
            Id = id;
            HasRole = hasRole;
            Name = name;
            Description = description;
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public bool HasRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}