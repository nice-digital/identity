using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

#if NET452
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NICE.Identity.Authentication.Sdk.Authentication
{
    public interface IAuthenticationService
    {
        Task Login(
#if NET452
		System.Web.HttpContext context,
#else
		Microsoft.AspNetCore.Http.HttpContext context,
#endif
		string returnUrl, bool goToRegisterPage = false);
        Task Logout(HttpContext context, string returnUrl);
    }
}