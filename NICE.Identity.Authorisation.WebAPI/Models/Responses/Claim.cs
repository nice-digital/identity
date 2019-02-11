namespace NICE.Identity.Authorisation.WebAPI.Models.Responses
{
    public class Claim
    {
	    public Claim(int roleId, string roleName)
	    {
		    RoleId = roleId;
		    RoleName = roleName;
	    }

	    public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}