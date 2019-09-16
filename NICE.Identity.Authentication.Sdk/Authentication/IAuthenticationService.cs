using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.Authentication
{
    public interface IAuthenticationService
    {
        Task Login(HttpContext context, string returnUrl);
        Task Logout(HttpContext context, string returnUrl);
    }
}