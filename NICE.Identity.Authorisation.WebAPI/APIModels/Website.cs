using System;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Website
    {

        public Website()
        {
        }
        
        public Website(int websiteId, int serviceId, int environmentId, string host)
        {
            WebsiteId = websiteId;
            ServiceId = serviceId;
            EnvironmentId = environmentId;
            Host = host;
        }
        
        public Website(DataModels.Website website)
        {
            WebsiteId = website.WebsiteId;
            ServiceId = website.ServiceId;
            EnvironmentId = website.EnvironmentId;
            Host = website.Host;
        }

        public int WebsiteId { get; set; }
        public int ServiceId { get; set; }
        public int EnvironmentId { get; set; }
        public string Host { get; set; }
    }
}
