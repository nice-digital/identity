using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Extensions;
using NICE.Identity.TestClient.Models;

namespace NICE.Identity.TestClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _apiIdentifier;
        private readonly string _authorisationServiceUri;
		private readonly string _authDomain;
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
        private readonly IAPIService _apiService;
        private readonly IApiToken _apiToken;
        private readonly ApiTokenClient _apiTokenClient;

        public HomeController(IConfiguration configuration, IHttpClientFactory clientFactory, IAPIService apiService, IApiToken apiToken, ApiTokenClient apiTokenClient)
        {
	        _configuration = configuration;
	        _clientFactory = clientFactory;
            _apiService = apiService;
            _apiToken = apiToken;
            _apiTokenClient = apiTokenClient;
            _apiIdentifier = configuration.GetSection("WebAppConfiguration").GetSection("ApiIdentifier").Value;
            _authorisationServiceUri = configuration.GetSection("WebAppConfiguration").GetSection("AuthorisationServiceUri").Value;
			_authDomain = configuration.GetSection("WebAppConfiguration").GetSection("Domain").Value;
        }

        public async Task<IActionResult> Index()
        {
	        try
	        {
		        var authConfiguration = new AuthConfiguration(_configuration, "WebAppConfiguration");
		        var m2mToken = await _apiToken.GetAccessToken(authConfiguration);

                var organisations = await _apiService.GetOrganisations(new List<int> { 1 }, m2mToken);
		        ViewData["Organisation1"] = JsonConvert.SerializeObject(organisations.FirstOrDefault());
	        }
	        catch (Exception ex)
	        {
		        ViewData["Organisation1"] = "error:" + ex.ToString();
	        }

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

            var currentUsersNameIdentifier = User.NameIdentifier();
            try
            {
	            var users = await _apiService.FindUsers(new List<string> {currentUsersNameIdentifier});
	            var firstUser = users.First();
	            ViewData["Users.NameIdentifier"] = firstUser.NameIdentifier;
	            ViewData["Users.DisplayName"] = firstUser.DisplayName;
	            ViewData["Users.EmailAddress"] = firstUser.EmailAddress;
            }
            catch (Exception ex)
            {
	            ViewData["Users.NameIdentifier"] = "error:" + ex.ToString();
            }

			try
            {
	            var roles = await _apiService.FindRoles(new List<string> {currentUsersNameIdentifier},
		            "dev-identitytestcore.nice.org.uk");

	            ViewData["Roles"] = JsonConvert.SerializeObject(roles);
            }
            catch (Exception ex)
            {
	            ViewData["Roles"] = "error:" + ex.ToString();
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> ApiData()
        {
            var client = _clientFactory.CreateClient("HttpClient");
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_apiIdentifier}/users"), //new Uri($"{_authorisationServiceUri}/api/users") , // new Uri($"{_apiIdentifier}/users"), //, //new Uri($"{_apiIdentifier}/users"), 
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

        [Authorize(Policy = "Editor")]
        public IActionResult EditorAction()
        {
	        ViewData["Message"] = "This action only available to someone with the role: Editor";

	        return View("Index");
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


        [Authorize]
        public async Task<IActionResult> GetM2MToken()
        {
	        var authConfiguration = new AuthConfiguration(_configuration, "WebAppConfiguration");
            var m2mToken = await _apiTokenClient.GetAccessToken(authConfiguration);

	        ViewData["M2MToken"] = m2mToken;


            return View("M2MToken");
        }

    }
}