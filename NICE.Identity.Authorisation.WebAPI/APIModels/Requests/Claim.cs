namespace NICE.Identity.Authorisation.WebAPI.ApiModels.Requests
{
    public class Claim
    {
        public string UserId { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }
    }
}