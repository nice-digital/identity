using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Service
    {
        public Service()
        {
        }
        
        public Service(int serviceId, string name)
        {
            ServiceId = serviceId;
            Name = name;
            Websites = new HashSet<Website>();
        }
        
        public Service(DataModels.Service service)
        {
            ServiceId = service.ServiceId;
            Name = service.Name;
            Websites = service.Websites.Select(w => new Website()
            {
                WebsiteId = w.WebsiteId,
                ServiceId = w.ServiceId,
                Host = w.Host,
                EnvironmentId = w.EnvironmentId,
                Environment = new Environment(w.Environment)
            });
        }

        [JsonPropertyName("id")]
        public int? ServiceId { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<Website> Websites { get; set; }
    }
}