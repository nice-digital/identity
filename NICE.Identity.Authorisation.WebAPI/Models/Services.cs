using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
    public partial class Services
    {

        public Services()
        {
            Websites = new HashSet<Websites>();
        }

		public Services(int serviceId, string name)
		{
			ServiceId = serviceId;
			Name = name;
			Websites = new HashSet<Websites>();
		}

		public int ServiceId { get; set; }
        public string Name { get; set; }

        public ICollection<Websites> Websites { get; set; }
    }
}
