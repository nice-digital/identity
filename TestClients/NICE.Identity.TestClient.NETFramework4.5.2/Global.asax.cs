using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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

			GlobalConfiguration.Configure(WebApiConfig.Register);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
