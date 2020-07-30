using System.Collections.Generic;

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

        public void UpdateFromApiModel(ApiModels.Organisation organisation)
        {
            OrganisationId = organisation?.OrganisationId ?? OrganisationId;
            Name = organisation?.Name ?? Name;
        }
    }
}
