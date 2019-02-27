using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authentication.Sdk.Abstractions;
using System.Threading.Tasks;

namespace NICE.Identity.TestClient.NETCore.Controllers
{
	public class AccountController : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAuthenticationService _niceAuthenticationService;

		public AccountController(IHttpContextAccessor httpContextAccessor, IAuthenticationService niceAuthenticationService)
		{
			_httpContextAccessor = httpContextAccessor;
			_niceAuthenticationService = niceAuthenticationService;
		}

		public async Task Login(string returnUrl = "/")
		{

			_httpContextAccessor.HttpContext.Session.SetString("_Name", "Test");
			_httpContextAccessor.HttpContext.Session.SetInt32("_Time", 773);
			await _niceAuthenticationService.Login(_httpContextAccessor.HttpContext, returnUrl);
		}

		[Authorize]
		public async Task Logout(string returnUrl = "/")
		{
			await _niceAuthenticationService.Logout(_httpContextAccessor.HttpContext, returnUrl);
		}
	}
}
