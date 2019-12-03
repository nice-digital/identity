using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class Service
    {

        public Service()
        {
            Websites = new HashSet<Website>();
        }

		public Service(int serviceId, string name)
		{
			ServiceId = serviceId;
			Name = name;
			Websites = new HashSet<Website>();
		}

		public int ServiceId { get; set; }
        public string Name { get; set; }

        public ICollection<Website> Websites { get; set; }

        public void UpdateFromApiModel(ApiModels.Service service)
        {
            Name = service?.Name ?? Name;
        }
    }
}
