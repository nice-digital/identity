using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using NICE.Identity.Authentication.Sdk.Configuration;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
    public class ScopeAuthoriseAttribute : System.Web.Http.AuthorizeAttribute
    {
        private readonly string scope;

        public ScopeAuthoriseAttribute(string scope)
        {
            this.scope = scope;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            var requestScope = actionContext.Request.GetDependencyScope();
            var auth0Configuration = requestScope.GetService(typeof(Auth0ServiceConfiguration)) as Auth0ServiceConfiguration;

            // Get the Auth0 domain, in order to validate the issuer
            var domain = $"https://{auth0Configuration.Domain}/";

            // Get the claim principal
            ClaimsPrincipal principal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;

            // Get the scope clain. Ensure that the issuer is for the correcr Auth0 domain
            var scopeClaim = principal?.Claims.FirstOrDefault(c => c.Type == "scope" && c.Issuer == domain);
            if (scopeClaim != null)
            {
                // Split scopes
                var scopes = scopeClaim.Value.Split(' ');

                // Succeed if the scope array contains the required scope
                if (scopes.Any(s => s == scope))
                    return;
            }

            HandleUnauthorizedRequest(actionContext);
        }
    }
}
