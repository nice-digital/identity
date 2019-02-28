using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NICE.Identity.TestClient.NETCore
{
    internal class ProductionDependencies
    {
        public static IServiceCollection AddProductionDependencies(IServiceCollection services, IConfigurationRoot configuration)
        { 
            return services;
        }
    }
}