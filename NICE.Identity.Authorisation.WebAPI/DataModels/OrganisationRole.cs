using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public class OrganisationRole
    {
        public OrganisationRole()
        {

        }

        public OrganisationRole(int organisationRoleId, int organisationId, int roleId)
        {
            OrganisationRoleId = organisationRoleId;
            OrganisationId = organisationId;
            RoleId = roleId;
        }

        public int OrganisationRoleId { get; set; }
        public int OrganisationId { get; set; }
        public int RoleId { get; set; }
        public Organisation Organisation { get; set; }
        public Role Role { get; set; }

        public void UpdateFromApiModel(ApiModels.OrganisationRole organisationRole)
        {
            OrganisationId = organisationRole?.OrganisationId ?? OrganisationId;
            RoleId = organisationRole?.RoleId ?? RoleId;
        }
    }
}
