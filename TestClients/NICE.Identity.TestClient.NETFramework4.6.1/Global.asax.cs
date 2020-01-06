using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NICE.Identity.TestClient.NETFramework;

namespace NICE.Identity.TestClient.NETFramework461
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
			GlobalConfiguration.Configure(WebApiConfig.Register);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
