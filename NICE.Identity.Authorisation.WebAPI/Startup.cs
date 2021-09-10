﻿using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.Authorisation.WebAPI.Configuration;
using NICE.Identity.Authorisation.WebAPI.Environments;
using NICE.Identity.Authorisation.WebAPI.HealthChecks;
using NICE.Identity.Authorisation.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Auth0.AuthenticationApi;
using Auth0.ManagementApi;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;
		private const string ApiTitle = "NICE Identity API";
		private const string ApiVersion = "v1";
        private const string ApiDescription = "NICE Identity API";

        private const string CorsPolicyName = "IdentityCorsPolicy";
        private const string APIKeyPolicyName = "APIKeyPolicy";

        private const string AuthenticatedHealthCheckTag = "authenticated";

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			_env = env;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			AppSettings.Configure(services, Configuration);
			var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<IdentityContext>(options =>
				options.UseSqlServer(sqlConnectionString));

			services.AddTransient<IClaimsService, ClaimsService>();
			services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IWebsitesService, WebsitesService>();
            services.AddTransient<IServicesService, ServicesService>();
            services.AddTransient<IEnvironmentsService, EnvironmentsService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IUserRolesService, UserRolesService>();
            services.AddTransient<IProviderManagementService, Auth0ManagementService>();
			services.AddTransient<IOrganisationsService, OrganisationsService>();
			services.AddTransient<IOrganisationRolesService, OrganisationRolesService>();
			services.AddTransient<IJobsService, JobsService>();
			services.AddTransient<IUserSyncCheck, UserSyncCheck>();
			services.AddTransient<IDuplicateCheck, DuplicateCheck>();
			services.AddSingleton<IManagementConnection, HttpClientManagementConnection>();
			services.AddTransient<IEmailService, EmailService>();
			services.AddHttpClient(); //this adds http client factory for use in DI

			services.AddRouting(options => options.LowercaseUrls = true);

			var authConfiguration = new AuthConfiguration(Configuration, "IdentityApiConfiguration");
			services.AddAuthentication(authConfiguration)
				.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler> (ApiKeyAuthenticationOptions.DefaultScheme, options => { }); //the health check uses an api key based scheme as it doesn't support oidc yet.

			services.AddAuthorisation(authConfiguration, authOptions =>
            {
	            authOptions.AddPolicy(APIKeyPolicyName, policyBuilder =>
				{
					policyBuilder.AuthenticationSchemes = new List<string> { ApiKeyAuthenticationOptions.DefaultScheme };
					policyBuilder.AddRequirements(new RoleRequirement(ApiKeyAuthenticationOptions.APIKeyRole));
				});
			});

			services.AddControllers();

			if (AppSettings.EnvironmentConfig.UseSwaggerUI)
			{
				services.AddSwaggerGen(options =>
				{
					options.SwaggerDoc(ApiVersion, new OpenApiInfo
					{
						Title = ApiTitle,
						Version = ApiVersion,
						Description = ApiDescription
					});
					var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
					options.IncludeXmlComments(xmlPath);
					options.AddSecurityDefinition(AuthenticationConstants.JWTAuthenticationScheme,
						new OpenApiSecurityScheme
						{
							In = ParameterLocation.Header,
							Description = "Enter into the field the word '" +
							              AuthenticationConstants.JWTAuthenticationScheme +
							              "' followed by space and the JWT token",
							Name = "Authorization",
							Type = SecuritySchemeType.ApiKey
						});
					options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = AuthenticationConstants.JWTAuthenticationScheme
								}
							},
							new string[] { }
						}
					});
				});

				services.ConfigureSwaggerGen(c => { c.CustomSchemaIds(x => x.FullName); });
			}

			IdentityModelEventSource.ShowPII = true;

			services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    builder =>
                    {
                        builder
	                        .SetIsOriginAllowedToAllowWildcardSubdomains()
	                        .WithOrigins("https://*.nice.org.uk", "https://local-identityadmin.nice.org.uk:44300")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
	                        .Build();
                    });
            });

			//health check code
			var healthChecksBuilder = services.AddHealthChecks();

			healthChecksBuilder.AddSqlServer(connectionString: sqlConnectionString,
				healthQuery: "SELECT 1;",
				name: "Identity database",
				failureStatus: HealthStatus.Degraded,
				tags: new[] { AuthenticatedHealthCheckTag });

			healthChecksBuilder.AddCheck<DuplicateCheck>("Duplicate email addresses in DB", tags: new[] { AuthenticatedHealthCheckTag });
			healthChecksBuilder.AddCheck<UserSyncCheck>("Users are synchronised between databases", tags: new[] { AuthenticatedHealthCheckTag });

			if (authConfiguration.RedisConfiguration.Enabled)
			{
				healthChecksBuilder.AddRedis(
					redisConnectionString: authConfiguration.RedisConfiguration.ConnectionString,
					name: "Redis",
					failureStatus: HealthStatus.Degraded,
					tags: new[] { AuthenticatedHealthCheckTag });
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostApplicationLifetime appLifetime, ILogger<Startup> startupLogger)
		{
			startupLogger.LogInformation("Identity WebAPI starting up");
			if (_env.IsDevelopment())
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
			app.UseStaticFiles(); //this line must be before UserRouting.
			//app.UseCookiePolicy();

			app.UseRouting();

			app.UseCors(CorsPolicyName); //cors should be before UseAuthentication and UseEndpoints

			app.UseAuthentication(); //this line must be in between UseRouting and UseEndpoints
			app.UseAuthorization();

			app.UseEndpoints(config =>
			{
				config.MapDefaultControllerRoute();

				config.MapHealthChecks(AppSettings.EnvironmentConfig.HealthCheckPublicAPIEndpoint, new HealthCheckOptions()
				{
					Predicate = (check) => !check.Tags.Contains(AuthenticatedHealthCheckTag),
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse					
				});

				config.MapHealthChecks(AppSettings.EnvironmentConfig.HealthCheckPrivateAPIEndpoint, new HealthCheckOptions
				{
					Predicate = _ => true,
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				}).RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme });
			});

			if (AppSettings.EnvironmentConfig.UseSwaggerUI)
			{
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiTitle);
					options.RoutePrefix = string.Empty; //this makes the route of the website use swagger
					options.DisplayOperationId();
					options.ShowExtensions();
				});
			}

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