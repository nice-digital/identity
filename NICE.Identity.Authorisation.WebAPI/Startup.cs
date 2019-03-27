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

	    private const string ApiSpecificOrigins = "_apiSpecificOrigins";

	    // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettings.Configure(services, Configuration,
                Environment.IsDevelopment() ? @"c:\" : Environment.ContentRootPath);
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.AddTransient<IManagementApiService, ManagementApiService>();
            services.AddTransient<IClaimsService, ClaimsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IJobsService, JobsService>();

            services.AddMvc()
	            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services
				.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(ApiVersion, new Info {Title = ApiTitle, Version = ApiVersion});
                })
                ;

            services.ConfigureSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
            });

			services.AddCors(options =>
			{
				options.AddPolicy(ApiSpecificOrigins,
					builder =>
					{
						builder.WithOrigins(AppSettings.ManagementAPI.CorsOrigins)
							.AllowCredentials()
							.AllowAnyHeader()
							.AllowAnyMethod();
					});
			});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime)
        {
	        seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
	        var startupLogger = loggerFactory.CreateLogger<Startup>();

			//enabling swagger on all environments for now. alpha thinks it's production even though it's got the environment variable set.
	        //if (!env.IsProduction())
	        //{
				app.UseSwagger();

		        app.UseSwaggerUI(c =>
		        {
			        c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiTitle);
			        c.RoutePrefix = string.Empty;
		        });
			//}


			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
			app.UseCors(ApiSpecificOrigins);
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