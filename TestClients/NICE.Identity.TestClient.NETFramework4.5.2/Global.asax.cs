using System.Net;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NICE.Identity.TestClient.NETFramework;

namespace NICE.Identity.TestClient.NETFramework452
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
			{
				ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
			}

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}
