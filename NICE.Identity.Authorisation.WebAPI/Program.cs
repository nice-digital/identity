using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NICE.Identity.Authorisation.WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}

	//public class Program
	//{
	//	public static void Main(string[] args)
	//	{
	//		Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory = env =>
	//			new ConfigurationBuilder()
	//				.SetBasePath(env.ContentRootPath)
	//				.AddEnvironmentVariables()
	//				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	//				//.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
	//				.AddUserSecrets(Assembly.GetAssembly(typeof(Startup)));

	//		var startup = new Startup("AuthorisationAPI", configurationFactory, ProductionDependencies.AddProductionDependencies);

	//		var builder = new WebHostBuilder()
	//			.UseKestrel()
	//			.UseContentRoot(Directory.GetCurrentDirectory())
	//			.ConfigureServices(services => services.TryAddSingleton<IStartup>(startup));

	//		using (var host = builder.Build())
	//		{
	//			host.Run();
	//		}
	//	}

	//}
}


