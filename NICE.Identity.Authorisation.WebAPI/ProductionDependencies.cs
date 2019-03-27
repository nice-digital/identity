using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Identity.Authorisation.WebAPI.Repositories;
using NICE.Identity.Authorisation.WebAPI.Services;

namespace NICE.Identity.Authorisation.WebAPI
{
	public static class ProductionDependencies
	{
		public static IServiceCollection AddProductionDependencies(IServiceCollection services, IConfigurationRoot configuration)
		{
			services.AddDbContext<IdentityContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

			services.TryAddSingleton<ISeriLogger, SeriLogger>();
			services.AddTransient<IClaimsService, ClaimsService>();
			services.AddTransient<IUsersService, UsersService>();

			return services;
		}
	}
}
