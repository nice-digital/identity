using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

namespace NICE.Identity.Authentication.Sdk.Authentication
{
    public interface IAuthenticationService
    {
        Task Login(HttpContext context, string returnUrl, bool goToRegisterPage = false);
        Task Logout(HttpContext context, string returnUrl);
    }
}