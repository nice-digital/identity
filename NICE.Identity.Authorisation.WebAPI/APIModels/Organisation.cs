using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Organisation
    {
        public Organisation()
        {

        }

        public Organisation(int organisationId, string name)
        {
            OrganisationId = organisationId;
            Name = name;
        }

        public Organisation(DataModels.Organisation organisation)
        {
            OrganisationId = organisation.OrganisationId;
            Name = organisation.Name;
        }


        [JsonPropertyName("id")]
        public int? OrganisationId { get; set; }
        public string Name { get; set; }
    }
}
