using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Requests
{
    public class CreateUser
    {
		/// <summary>
		/// this is the auth0userid
		/// </summary>
        [Required]
		[JsonProperty("UserId")]
        public string Auth0UserId { get; set; }

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
	    public bool InitialAllowContactMe { get; set; }
	}
}