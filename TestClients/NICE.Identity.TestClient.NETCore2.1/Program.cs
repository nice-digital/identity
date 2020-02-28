using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NICE.Identity.TestClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
#if NETCOREAPP
			CreateWebHostBuilder(args).Build().Run();
#endif
		}

#if NETCOREAPP
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
#endif
	}
}