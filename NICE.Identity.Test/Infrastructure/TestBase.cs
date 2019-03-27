using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Identity.Authorisation.WebAPI;
using System;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Test.Infrastructure
{
	public class TestBase
	{
		private TestServer _server;
		private HttpClient _client;
		private IdentityContext _context;
		
		public TestBase()
		{
			InitialiseDefaultServerAndClientWithoutAddingDatabaseRecords();
		}

		private void InitialiseDefaultServerAndClientWithoutAddingDatabaseRecords()
		{
			_context = GetContext();
			var serverAndClient = InitialiseServerAndClient(_context);
			_server = serverAndClient.testServer;
			_client = serverAndClient.httpClient;
		}

		private static (TestServer testServer, HttpClient httpClient) InitialiseServerAndClient(IdentityContext identityContext)
		{
			Func<IHostingEnvironment, IConfigurationBuilder> configurationFactory = env =>
				new ConfigurationBuilder()
					.SetBasePath(env.ContentRootPath)
					.AddEnvironmentVariables()
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					//.AddJsonFile($"appsettings.{environmentName}.json", optional: true)
					.AddUserSecrets(Assembly.GetAssembly(typeof(Startup)));


			var startup = new Startup("TestApp", configurationFactory, ConfigureVariantServices);

			var builder = new WebHostBuilder()
				.UseContentRoot("../../../../NICE.Identity")
				.ConfigureServices(services =>
				{
					services.AddSingleton<IStartup>(startup);
					services.TryAddTransient<IdentityContext>(provider => identityContext);
				})
				.Configure(app =>
				{
					app.UseStaticFiles();
				})
				.UseEnvironment("Production");
			var server = new TestServer(builder);
			return (testServer: server, httpClient: server.CreateClient());
		}

		private static IServiceCollection ConfigureVariantServices(IServiceCollection services, IConfigurationRoot configurationRoot)
		{
			return services;
		}

		protected HttpClient GetClient(IdentityContext identityContext = null)
		{
			if (identityContext == null)
				return _client;

			return InitialiseServerAndClient(identityContext).httpClient;
		}

		protected IdentityContext GetContext()
		{
			var databaseName = "TestIdentityDB" + Guid.NewGuid();
			var dbOptions = new DbContextOptionsBuilder<IdentityContext>()
				.UseInMemoryDatabase(databaseName)
				.Options;
			return new IdentityContext(dbOptions);
		}
	}
}
