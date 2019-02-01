using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NICE.Identity.Configuration;
using NICE.Identity.Models;
using NICE.Identity.Services;

namespace NICE.Identity
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			AppSettings.Configure(services, Configuration, Environment.IsDevelopment() ? @"c:\" : Environment.ContentRootPath);

			var connectionString = Configuration.GetConnectionString("DefaultConnection");
			if (!string.IsNullOrEmpty(connectionString) && !connectionString.StartsWith("NULL")
			) //todo: improve this. connection string is in secrets.json, don't save it in the appsettings.json. but it's useful having something there so the code doesn't fall over looking for it, and so that people can learn where it should be set.
			{
				services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
			}

			//dependency injection goes here.
			services.TryAddSingleton<ISeriLogger, SeriLogger>();
			services.TryAddTransient<IAuditService, AuditService>();


			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/build";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime)
		{
			seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
			loggerFactory.CreateLogger<Startup>();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				loggerFactory.AddConsole(Configuration.GetSection("Logging"));
				loggerFactory.AddDebug();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
				app.UseStatusCodePagesWithReExecute("/error/{0}"); // url to errorcontroller
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "adminRoute",
					template: "{area:exists}/{controller=home}/{action=index}/{id?}"
				);

				routes.MapRoute(
					name: "default",
					template: "{controller=home}/{action=index}/{id?}"
				);
			});

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					spa.UseReactDevelopmentServer(npmScript: "start");
				}
			});
		}
	}
}
