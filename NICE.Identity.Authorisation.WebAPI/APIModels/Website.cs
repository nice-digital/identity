using Newtonsoft.Json;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Website
    {

        public Website()
        {
        }
        
        public Website(int websiteId, int serviceId, int environmentId, string host, Environment environment)
        {
            WebsiteId = websiteId;
            ServiceId = serviceId;
            EnvironmentId = environmentId;
            Host = host;
            Environment = environment;
        }
        
        public Website(DataModels.Website website)
        {
            WebsiteId = website.WebsiteId;
            ServiceId = website.ServiceId;
            EnvironmentId = website.EnvironmentId;
            Host = website.Host;
            Environment = website.Environment != null ? new Environment(website.Environment) : null;
        }

        [JsonProperty(PropertyName = "id")]
        public int? WebsiteId { get; set; }
        public int? ServiceId { get; set; }
        public int? EnvironmentId { get; set; }
        public string Host { get; set; }
        public Environment Environment { get; set; }
    }
}
