using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NICE.Identity.Authentication.Sdk.Authentication;
using System.Threading.Tasks;

namespace NICE.Identity.TestClient.Controllers
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

		public async Task Login(string returnUrl = "/", bool goToRegisterPage = false)
		{
#if NETCOREAPP
			await _niceAuthenticationService.Login(_httpContextAccessor.HttpContext, returnUrl, goToRegisterPage);
#endif
		}

		[Authorize]
		public async Task Logout(string returnUrl = "/")
		{
#if NETCOREAPP
			await _niceAuthenticationService.Logout(_httpContextAccessor.HttpContext, returnUrl);
#endif
		}
	}
}
