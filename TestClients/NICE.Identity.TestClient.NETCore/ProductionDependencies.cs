using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace NICE.Identity.TestClient.NetCore
{
    internal class ProductionDependencies
    {
        public static IServiceCollection AddProductionDependencies(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Sample API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme {In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey"});
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()},
                });
            });

            services.AddMvcCore()
                    .AddApiExplorer();

            return services;
        }
    }
}
