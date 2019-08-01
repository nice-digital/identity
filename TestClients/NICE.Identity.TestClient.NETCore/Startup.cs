using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.TestClient.NetCore
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		//private const string AuthorisationServiceConfigurationPath = "AuthorisationServiceConfiguration";
		private const string RedisServiceConfigurationPath = "RedisServiceConfiguration";

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
			//	options.CheckConsentNeeded = context => true;
			//	options.MinimumSameSitePolicy = SameSiteMode.None;
			//});
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddAuthenticationSdk(new AuthConfiguration(Configuration, "AuthConfiguration"));
			services.AddRedisCacheSDK(Configuration, RedisServiceConfigurationPath, "todo:somestringforredis");
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
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			//app.UseCookiePolicy();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}


	//public class HarryStartup : CoreStartUpBase
	//{
	//	public HarryStartup(Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory,
	//	               Func<IServiceCollection, IConfigurationRoot, IServiceCollection> configureVariantServices)
	//		: base("Default App", configurationFactory, configureVariantServices) { }

	//	protected override IServiceCollection  ConfigureInvariantServices(IServiceCollection services, IHostingEnvironment env, IConfigurationRoot configuration)
	//	{
	//		services.Configure<CookiePolicyOptions>(options =>
	//		{
	//			// This lambda determines whether user consent for non-essential cookies is needed for a given request.
	//			options.CheckConsentNeeded = context => true;
	//			options.MinimumSameSitePolicy = SameSiteMode.None;
	//		});

	//		services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
	//		services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

	//		return services;
	//	}

	//	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	//	public override void Configure(IApplicationBuilder app)
	//	{
	//		if (environment.IsDevelopment())
	//		{
	//			app.UseDeveloperExceptionPage();
	//		}
	//		else
	//		{
	//			app.UseExceptionHandler("/Home/Error");
	//			app.UseHsts();
	//		}

	//		app.UseHttpsRedirection();
	//		app.UseStaticFiles();
	//		app.UseCookiePolicy();
	//		app.UseSwagger();
	//		app.UseSwaggerUI(c =>
	//		{
	//			c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API V1");
	//		});

 //           app.UseMvc(routes =>
	//		{
	//			routes.MapRoute(
	//				name: "default",
	//				template: "{controller=Home}/{action=Index}/{id?}");
	//		});
	//	}
	//}
}
