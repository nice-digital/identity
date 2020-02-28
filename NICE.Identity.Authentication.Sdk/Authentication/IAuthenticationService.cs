using System.Collections.Generic;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Domain;

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
            string returnUrl, 
            bool goToRegisterPage = false
        );      
        
        
        Task Logout(
#if NET452
            System.Web.HttpContext context,
#else
			Microsoft.AspNetCore.Http.HttpContext context,
#endif
            string returnUrl
        );
    }
}