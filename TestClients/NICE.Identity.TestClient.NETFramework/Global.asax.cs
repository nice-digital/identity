using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.TestClient.NETFramework
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
