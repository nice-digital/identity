using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.TestClient.NETCore.Authorisation;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace NICE.Identity.TestClient.NETCore
{
	public class Startup
	{
	    private const string AuthorisationServiceConfigurationPath = "AuthorisationServiceConfiguration";
	    private const string AuthenticationServiceConfigurationPath = "AuthenticationServiceConfiguration";

        public Startup(IConfiguration configuration, IHostingEnvironment env)
	    {
	        var builder = new ConfigurationBuilder()
	            .SetBasePath(env.ContentRootPath)
	            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
	            .AddEnvironmentVariables();
	        Configuration = builder.Build();
	    }

        public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddAuthorization(options =>
			{
				//options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrator"));
				options.AddPolicy("RequireAdminRole", policy => policy.Requirements.Add(new RoleRequirement("Administrator")));
			});
		    services.AddAuthenticationSdk(Configuration, AuthorisationServiceConfigurationPath, AuthenticationServiceConfigurationPath);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
