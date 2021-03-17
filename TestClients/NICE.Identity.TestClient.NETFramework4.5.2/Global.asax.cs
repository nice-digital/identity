using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
//using Autofac;
//using Autofac.Integration.Mvc;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.TestClient.NETFramework;

namespace NICE.Identity.TestClient.NETFramework452
{

	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Attribute routing.
			config.MapHttpAttributeRoutes();
		}
	}

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
			{
				ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
			}


			//var builder = new Autofac.ContainerBuilder();
			//builder.RegisterControllers(typeof(MvcApplication).Assembly);

			//builder.RegisterInstance<IAuthConfiguration>(authConfiguration);
			//Container = builder.Build();

			//var redisConfig = new RedisConfiguration
			//{
			//	IpConfig = secretsFile.SelectToken("RedisServiceConfiguration")["IpConfig"].ToString(),
			//	Port = int.Parse(secretsFile.SelectToken("RedisServiceConfiguration")["Port"].ToString()),
			//	Enabled = bool.Parse(secretsFile.SelectToken("RedisServiceConfiguration")["Enabled"].ToString())
			//};


			GlobalConfiguration.Configure(WebApiConfig.Register);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
