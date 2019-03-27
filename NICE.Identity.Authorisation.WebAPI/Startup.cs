using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk;
using NICE.Identity.Authentication.Sdk.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI
{
    public class Startup :CoreStartUpBase
    {
        private const string ApiTitle = "NICE.Identity.Authorisation.WebAPI";
        private const string ApiVersion = "v1";

	    public Startup(string clientName, 
			Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory, 
			Func<IServiceCollection, IConfigurationRoot, IServiceCollection> configureVariantServices) : base(clientName, configurationFactory, configureVariantServices)
	    {

	    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime)
        {
	        seriLogger.Configure(loggerFactory, configuration, appLifetime, env);
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

	    protected override IServiceCollection ConfigureInvariantServices(IServiceCollection services, IHostingEnvironment env,
		    IConfigurationRoot configuration)
	    {
		    services.AddMvc()
			    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

		    services
			    .AddSwaggerGen(c =>
			    {
				    c.SwaggerDoc(ApiVersion, new Info { Title = ApiTitle, Version = ApiVersion });
			    })
			    ;

		    services.ConfigureSwaggerGen(c =>
		    {
			    c.CustomSchemaIds(x => x.FullName);
		    });

		    services.AddMvcCore()
			    .AddApiExplorer();

			return services;
	    }
    }
}