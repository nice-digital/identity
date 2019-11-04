using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NICE.Identity.TestClient.NetCore.Models;

namespace NICE.Identity.TestClient.NetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _apiIdentifier;
        private readonly string _authDomain;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _apiIdentifier = configuration.GetSection("WebAppConfiguration").GetSection("ApiIdentifier").Value;
            _authDomain = configuration.GetSection("WebAppConfiguration").GetSection("Domain").Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            ViewData["IdToken"] = await HttpContext.GetTokenAsync("id_token");
            ViewData["AccessToken"] = await HttpContext.GetTokenAsync("access_token");
            ViewData["AccessTokenExpires"] = await HttpContext.GetTokenAsync("expires_at");
            ViewData["TokenType"] = await HttpContext.GetTokenAsync("token_type");
            ViewData["RefreshToken"] = await HttpContext.GetTokenAsync("refresh_token");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ApiData()
        {
            var client = _clientFactory.CreateClient("HttpClient");
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_apiIdentifier}/users"),
                Method = HttpMethod.Get,
                Headers = {Authorization = new AuthenticationHeaderValue("Bearer", accessToken)}
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var usersResponse = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserViewModel>>(usersResponse);
                return View(users);
            }

            return View("Error");
        }

        [Authorize]
        public IActionResult UserProfileScoped()
        {
            ViewData["Message"] = "Your application description page.";

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}