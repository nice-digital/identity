using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using NICE.Identity.Authorisation.WebAPI.Services;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI
{
    public class Startup
    {
        private const string ApiTitle = "NICE.Identity.Authorisation.WebAPI";
        private const string ApiVersion = "v1";

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
			services.AddDbContext<IdentityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));       

	        services.TryAddSingleton<ISeriLogger, SeriLogger>();
	        services.AddTransient<IClaimsService, ClaimsService>();

			services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(ApiVersion, new Info {Title = ApiTitle, Version = ApiVersion});
                })
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime)
        {
	        seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
	        var startupLogger = loggerFactory.CreateLogger<Startup>();

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiTitle);
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

	        try
	        {
		        var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
		        serviceScope.ServiceProvider.GetService<IdentityContext>().Database.Migrate();
		        serviceScope.Dispose();
	        }
	        catch (Exception ex)
	        {
		        startupLogger.LogError($"EF Migrations Error: {ex}");
	        }
		}
    }
}