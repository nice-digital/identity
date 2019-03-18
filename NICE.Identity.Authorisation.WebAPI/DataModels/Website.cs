using System;
using System.Collections.Generic;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class Website
    {

        public Website()
        {
            Roles = new HashSet<Role>();
        }

		public Website(int websiteId, int serviceId, int environmentId, string host)
		{
			WebsiteId = websiteId;
			ServiceId = serviceId;
			EnvironmentId = environmentId;
			Host = host;
			Roles = new HashSet<Role>();
		}

		public int WebsiteId { get; set; }
        public int ServiceId { get; set; }
        public int EnvironmentId { get; set; }
        public string Host { get; set; }

        public Environment Environment { get; set; }
        public Service Service { get; set; }
        public ICollection<Role> Roles { get; set; }
    }
}
