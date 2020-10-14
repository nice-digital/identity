using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace NICE.Identity.Authorisation.WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = SeriLogger.GetLoggerConfiguration().CreateLogger();
			try
			{
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Application start-up failed");
			}
			finally
			{
				Log.CloseAndFlush();
			}			
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog() 
				.ConfigureWebHostDefaults(webBuilder =>
				{
					//webBuilder.UseKestrel()
					//   .ConfigureKestrel(serverOptions =>
					//   {
					//	   serverOptions.ListenAnyIP(6969);
					//   });
					webBuilder.UseStartup<Startup>();
				});
	}
}