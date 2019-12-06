using System.Collections.Generic;

namespace NICE.Identity.Authorisation.WebAPI.DataModels
{
    public partial class Environment
    {

        public Environment()
        {
            Websites = new HashSet<Website>();
        }

		public Environment(int environmentId, string name, int order = 0) : this ()
		{
			EnvironmentId = environmentId;
			Name = name;
		}

		public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public ICollection<Website> Websites { get; set; }
        
        public void UpdateFromApiModel(ApiModels.Environment environment)
        {
            Name = environment?.Name ?? Name;
            Order = environment?.Order ?? Order;
        }
    }
}
