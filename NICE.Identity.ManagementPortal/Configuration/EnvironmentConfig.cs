namespace NICE.Identity.ManagementPortal.Configuration
{
    public class EnvironmentConfig
    {
        private const string LocalEnvironmentName = "local";

        public string Name { get; set; }

        //public bool IsLocal => string.Equals(Name, LocalEnvironmentName, StringComparison.OrdinalIgnoreCase);
    }
}
