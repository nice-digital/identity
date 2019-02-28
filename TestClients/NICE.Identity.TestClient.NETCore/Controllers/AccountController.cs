using System.Linq;
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
			await _niceAuthenticationService.Login(_httpContextAccessor.HttpContext, returnUrl);

			if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var accessToken = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                _httpContextAccessor.HttpContext.Session.SetString("access_token", accessToken);
            }
		}

		[Authorize]
		public async Task Logout(string returnUrl = "/")
		{
			await _niceAuthenticationService.Logout(_httpContextAccessor.HttpContext, returnUrl);
		}
	}
}
