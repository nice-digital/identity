namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
	public class EnvironmentConfig
	{
		public string Name { get; set; }
		public bool UseSwaggerUI { get; set; }
		public string HealthCheckPublicAPIEndpoint { get; set; } = "healthcheckapi-local";
		public string HealthCheckPrivateAPIEndpoint { get; set; } = "healthcheckapi-private";
		public string HealthCheckPrivateAPIKey { get; set; }
	}
}
