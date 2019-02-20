using System.ComponentModel.DataAnnotations;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Requests
{
    public class User
    {
        [Required]
        public string UserId { get; set; }

		public string Title { get; set; }

        [Required]
		[StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

	    [Required]
	    public bool AcceptedTerms { get; set; }

		[Required]
	    public bool AllowContactMe { get; set; }

		public bool IsStaffMember { get; set; }
	}
}