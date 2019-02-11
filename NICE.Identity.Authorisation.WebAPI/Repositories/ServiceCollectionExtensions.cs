using Microsoft.Extensions.DependencyInjection;
using NICE.Identity.Authorisation.WebAPI.Abstractions;

namespace NICE.Identity.Authorisation.WebAPI.Repositories
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}