using System.ComponentModel.DataAnnotations;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Requests
{
    public class User
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
    }
}