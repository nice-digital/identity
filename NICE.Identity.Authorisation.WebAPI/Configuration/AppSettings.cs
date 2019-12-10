using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
 
namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public static class AppSettings
    {
        // this is a static class for storing appsettings so we don't have to use DI for passing configuration stuff
        // (i.e. to stop us having to pass IOptions<SomeConfig> through the stack)

        public static ManagementAPIConfig ManagementAPI { get; private set; }
        public static EnvironmentConfig EnvironmentConfig { get; private set; }

		public static void Configure(IServiceCollection services, IConfiguration configuration)
         {
             services.Configure<ManagementAPIConfig>(configuration.GetSection("Auth0ManagementApiConfiguration"));
             services.Configure<EnvironmentConfig>(configuration.GetSection("Environment"));
			
             var sp = services.BuildServiceProvider();

             ManagementAPI = sp.GetService<IOptions<ManagementAPIConfig>>().Value;
             EnvironmentConfig = sp.GetService<IOptions<EnvironmentConfig>>().Value;
		}
    }
}
