using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NICE.Identity.TestClient.M2MApp.Configuration
{
    public static class AppSettings 
    {
        // this is a static class for storing appsettings so we don't have to use DI for passing configuration stuff
        // (i.e. to stop us having to pass IOptions<SomeConfig> through the stack)

        public static Auth0Config Auth0Config { get; set; }
       
		public static void Configure(IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.Configure<Auth0Config>(configuration.GetSection("Auth0"));
            
			var sp = services.BuildServiceProvider();
            Auth0Config = sp.GetService<IOptions<Auth0Config>>().Value;
		}
    }
}
