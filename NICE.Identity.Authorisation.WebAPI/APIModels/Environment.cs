using System.Collections.Generic;
using Newtonsoft.Json;

namespace NICE.Identity.Authorisation.WebAPI.ApiModels
{
    public class Environment
    {
        public Environment()
        {
        }
        public Environment(int environmentId, string name, int order = 0)
        {
            EnvironmentId = environmentId;
            Name = name;
        }
        
        public Environment(DataModels.Environment environment)
        {
            EnvironmentId = environment.EnvironmentId;
            Name = environment.Name;
            Order = environment.Order;
        }

        public int EnvironmentId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
