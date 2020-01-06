using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NICE.Identity.TestClient.NETFramework;

namespace NICE.Identity.TestClient.NETFramework461
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
