using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NICE.Identity.Authorisation.WebAPI.Configuration
{
    public static class AppSettings 
    {
        // this is a static class for storing appsettings so we don't have to use DI for passing configuration stuff
        // (i.e. to stop us having to pass IOptions<SomeConfig> through the stack)

        public static AuthorisationAPIConfig AuthorisationAPI { get; private set; }
       
		public static void Configure(IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.Configure<AuthorisationAPIConfig>(configuration.GetSection("AuthorisationAPI"));
            
			var sp = services.BuildServiceProvider();
	        AuthorisationAPI = sp.GetService<IOptions<AuthorisationAPIConfig>>().Value;
		}
    }
}
