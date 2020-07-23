using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public class Organisation
    {
        public Organisation()
        {
            OrganisationRoles = new HashSet<OrganisationRole>();
        }

        public Organisation(int organisationId, string name)
        {
            OrganisationId = organisationId;
            Name = name;
        }

        public  int OrganisationId { get; set; }
        public string Name { get; set; }
        public ICollection<OrganisationRole> OrganisationRoles { get; set; }
        public ICollection<Job> Jobs { get; set; }

        public void UpdateFromApiModel(APIModels.Organisation organisation)
        {
            OrganisationId = organisation?.OrganisationId ?? OrganisationId;
            Name = organisation?.Name ?? Name;
        }
    }
}
