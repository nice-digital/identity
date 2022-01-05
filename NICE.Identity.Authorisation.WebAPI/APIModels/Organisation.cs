using System;
using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Organisation
    {
        public Organisation()
        {
        }

        public Organisation(int organisationId, string name, DateTime datedAdded)
        {
            OrganisationId = organisationId;
            Name = name;
            DatedAdded = DatedAdded;
        }

        public Organisation(DataModels.Organisation organisation)
        {
            OrganisationId = organisation.OrganisationId;
            Name = organisation.Name;
            DatedAdded = organisation.DateAdded;
        }

        [JsonPropertyName("id")]
        public int? OrganisationId { get; set; }
        public string Name { get; set; }
        public DateTime? DatedAdded { get; set; }
    }
}
