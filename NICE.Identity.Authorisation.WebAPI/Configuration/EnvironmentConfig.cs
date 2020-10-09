namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
	public class EnvironmentConfig
	{
		public string Name { get; set; }
		public bool UseSwaggerUI { get; set; }
		public string HealthChecksAPIEndpoint { get; set; } = "healthcheckapi-local";
		public bool UseHealthChecksUI { get; set; }
	}
}
