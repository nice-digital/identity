using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NICE.Identity.Models;
using System;
using System.Net.Http;


namespace NICE.Identity.Test.Infrastructure
{
	public class TestBase
	{
		private TestServer _server;
		private HttpClient _client;
		private IdentityContext _context;

		public TestBase()
		{
			InitialiseClient();
		}

		private void InitialiseClient()
		{
			var databaseName = "TestIdentityDB" + Guid.NewGuid();
			var dbOptions = new DbContextOptionsBuilder<IdentityContext>()
				.UseInMemoryDatabase(databaseName)
				.Options;

			_context = new IdentityContext(dbOptions);
			var builder = new WebHostBuilder()
				//.UseContentRoot("../../../../Comments")
				.ConfigureServices(services =>
				{

					services.TryAddSingleton<IdentityContext>(_context);

				})
				.Configure(app =>
				{
					//app.UseStaticFiles();

					//app.Use((context, next) =>
					//{
					//	var httpRequestFeature = context.Features.Get<IHttpRequestFeature>();

					//	if (httpRequestFeature != null && string.IsNullOrEmpty(httpRequestFeature.RawTarget))
					//		httpRequestFeature.RawTarget = httpRequestFeature.Path;

					//	return next();
					//});

				})
				.UseEnvironment("Production")
				.UseStartup(typeof(Startup));
			_server = new TestServer(builder);
			_client = _server.CreateClient();
		}

		protected HttpClient GetClient()
		{
			return _client;
		}

	}
}
