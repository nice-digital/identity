using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Job
    {
        public Job()
        {
        }

        public Job(DataModels.Job job)
        {
            JobId = job.JobId;
            UserId = job.UserId;
            OrganisationId = job.OrganisationId;
            IsLead = job.IsLead;
        }

        [JsonPropertyName("id")]
        public int? JobId { get; set; }
        public int? UserId { get; set; }
        public int? OrganisationId { get; set; }
        public bool? IsLead { get; set; }
    }

}
