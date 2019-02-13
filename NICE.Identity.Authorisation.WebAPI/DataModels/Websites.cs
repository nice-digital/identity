using System;
using System.Collections.Generic;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class Websites
    {

        public Websites()
        {
            Roles = new HashSet<Roles>();
        }

		public Websites(int websiteId, int serviceId, int environmentId, string host)
		{
			WebsiteId = websiteId;
			ServiceId = serviceId;
			EnvironmentId = environmentId;
			Host = host;
			Roles = new HashSet<Roles>();
		}

		public int WebsiteId { get; set; }
        public int ServiceId { get; set; }
        public int EnvironmentId { get; set; }
        public string Host { get; set; }

        public Environments Environment { get; set; }
        public Services Service { get; set; }
        public ICollection<Roles> Roles { get; set; }
    }
}
