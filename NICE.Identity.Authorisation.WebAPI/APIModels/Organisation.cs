using System;
using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Organisation
    {
        public Organisation()
        {
        }

        public Organisation(int organisationId, string name, DateTime dateAdded)
        {
            OrganisationId = organisationId;
            Name = name;
            DateAdded = dateAdded;
        }

        public Organisation(DataModels.Organisation organisation)
        {
            OrganisationId = organisation.OrganisationId;
            Name = organisation.Name;
            DateAdded = organisation.DateAdded;
        }

        [JsonPropertyName("id")]
        public int? OrganisationId { get; set; }
        public string Name { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
