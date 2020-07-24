using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class OrganisationRole
    {
        public OrganisationRole()
        {
        }

        public OrganisationRole(DataModels.OrganisationRole organisationRole)
        {
            OrganisationRoleId = organisationRole.OrganisationRoleId;
            OrganisationId = organisationRole.OrganisationId;
            RoleId = organisationRole.RoleId;
        }

        [JsonPropertyName("id")]
        public int? OrganisationRoleId { get; set; }
        public int? OrganisationId { get; set; }
        public int? RoleId { get; set; }
    }

}
