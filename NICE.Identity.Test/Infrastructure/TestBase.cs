using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;


namespace NICE.Identity.Test.Infrastructure
{
	public class TestBase
	{
		private TestServer _server;
		private HttpClient _client;
		//private IdentityContext _context;

		//public TestBase()
		//{
		//	InitialiseDefaultServerAndClientWithoutAddingDatabaseRecords();
		//}

		//private void InitialiseDefaultServerAndClientWithoutAddingDatabaseRecords()
		//{
		//	_context = GetContext();
		//	var serverAndClient = InitialiseServerAndClient(_context);
		//	_server = serverAndClient.testServer;
		//	_client = serverAndClient.httpClient;
		//}

		//private static (TestServer testServer, HttpClient httpClient) InitialiseServerAndClient(IdentityContext identityContext)
		//{
		//	var builder = new WebHostBuilder()
		//		.UseContentRoot("../../../../NICE.Identity")
		//		.ConfigureServices(services =>
		//		{
		//			services.TryAddTransient<IdentityContext>(provider => identityContext); //note: not a singleton like in the main code. 
		//		})
		//		.Configure(app =>
		//		{
		//			app.UseStaticFiles();
		//		})
		//		.UseEnvironment("Production")
		//		.UseStartup(typeof(Startup));
		//	var server = new TestServer(builder);
		//	return (testServer: server, httpClient: server.CreateClient());
		//}

		//protected HttpClient GetClient(IdentityContext identityContext = null)
		//{
		//	if (identityContext == null)
		//		return _client;

		//	return InitialiseServerAndClient(identityContext).httpClient;
		//}

		//protected IdentityContext GetContext()
		//{
		//	var databaseName = "TestIdentityDB" + Guid.NewGuid();
		//	var dbOptions = new DbContextOptionsBuilder<IdentityContext>()
		//		.UseInMemoryDatabase(databaseName)
		//		.Options;
		//	return new IdentityContext(dbOptions);
		//}

	}
}
