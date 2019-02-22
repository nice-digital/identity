using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.TestClient.Consumer.NETCore.Models;
using NICE.Identity.TestClient.M2MApp.Services;

namespace NICE.Identity.TestClient.M2MApp.Controllers
{
	public class HomeController : Controller
	{
	    private readonly ITestClientApiService _apiService;

	    public HomeController(ITestClientApiService apiService)
	    {
	        _apiService = apiService;
	    }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

	    public IActionResult Publication()
	    {
	        var publication = _apiService.GetPublication().Result;

	        ViewData["Id"] = publication.Id;
	        ViewData["Text"] = publication.SomeText;

	        return View();
	    }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
