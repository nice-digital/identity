using System;
using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.Models
{
    public partial class Environments
    {

        public Environments()
        {
            Websites = new HashSet<Websites>();
        }

		public Environments(int environmentId, string name)
		{
			EnvironmentId = environmentId;
			Name = name;
			Websites = new HashSet<Websites>();
		}

		public int EnvironmentId { get; set; }
        public string Name { get; set; }

        public ICollection<Websites> Websites { get; set; }
    }
}
