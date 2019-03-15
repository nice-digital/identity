using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.Authentication.Sdk
{
    public abstract class CoreStartUpBase : ICoreStartUpBase
    {
        protected readonly Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory;
        protected readonly Func<IServiceCollection, IConfigurationRoot, IServiceCollection> configureVariantServices;
        protected IHostingEnvironment environment;

        protected CoreStartUpBase(Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory,
                                  Func<IServiceCollection, IConfigurationRoot, IServiceCollection> configureVariantServices)
        {
            this.configurationFactory = configurationFactory;
            this.configureVariantServices = configureVariantServices;
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var tempServiceProvider = services.BuildServiceProvider();
            environment = tempServiceProvider.GetService<IHostingEnvironment>();
            var configuration = configurationFactory(environment).Build();

            services.AddAuthenticationSdk(configuration, "AuthorisationServiceConfiguration");

            configureVariantServices(services, configuration);

            ServiceProvider = ConfigureInvariantServices(services, environment, configuration).BuildServiceProvider();

            return ServiceProvider;
        }

        /// <summary>
        /// Register dependencies that will never be mocked. Invariant services are those that will always
        /// be used, even if we are running some test flavour.
        /// </summary>
        protected abstract IServiceCollection ConfigureInvariantServices(IServiceCollection services, IHostingEnvironment env, IConfigurationRoot configuration);

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}