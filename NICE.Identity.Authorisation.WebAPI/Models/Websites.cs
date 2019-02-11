using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
    public partial class Websites
    {
        public Websites()
        {
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
