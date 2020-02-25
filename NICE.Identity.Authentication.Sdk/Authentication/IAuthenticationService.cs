using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;
using System.Collections.Generic;

#if NETFRAMEWORK
using System.Web;
#elif NETCOREAPP || NETSTANDARD
using Microsoft.AspNetCore.Http;
#endif

namespace NICE.Identity.Authentication.Sdk.Authentication
{
    public interface IAuthenticationService
    {
        Task Login(HttpContext context, string returnUrl, bool goToRegisterPage = false);
        Task Logout(HttpContext context, string returnUrl);
    }
}