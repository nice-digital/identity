﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NICE.Identity.TestClient.M2MApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory = env =>
				new ConfigurationBuilder()
					.SetBasePath(env.ContentRootPath)
					.AddEnvironmentVariables()
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					//.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
					.AddUserSecrets(Assembly.GetAssembly(typeof(Startup)));

			var startup = new Startup(configurationFactory, ProductionDependencies.AddProductionDependencies);

			var builder = new WebHostBuilder()
			              .UseKestrel()
			              .UseContentRoot(Directory.GetCurrentDirectory())
			              .ConfigureServices(services => services.TryAddSingleton<IStartup>(startup));

			using (var host = builder.Build())
			{
				host.Run();
			}
        }
	}
}