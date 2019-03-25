using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nice.Identity.TestClient.MVC
{
    internal class ProductionDependencies
    {
        public static IServiceCollection AddProductionDependencies(IServiceCollection services, IConfigurationRoot configuration)
        {
            return services;
        }
    }
}
