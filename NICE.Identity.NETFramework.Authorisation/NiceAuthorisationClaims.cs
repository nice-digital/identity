using System.Linq;
using System.Security.Principal;
using Microsoft.IdentityModel.Claims;

namespace NICE.Identity.NETFramework.Authorisation
{
    internal static class NiceAuthorisationClaims
    {
        internal static ClaimResults ProcessClaims(IPrincipal principal, IAuthorisationService authService, string[] rolesRequested)
        {

            ClaimResults result = new ClaimResults {Result = ClaimResult.Failed};
            var claims = ((ClaimsIdentity) principal.Identity).Claims;

            result.IdClaim = claims.FirstOrDefault(claim => claim.ClaimType.Equals(ClaimTypes.NameIdentifier));
            result.Name = principal.Identity.Name;

            if (result.IdClaim != null)
            {
                if (!rolesRequested.Any() || authService.UserSatisfiesAtLeastOneRole(result.IdClaim.Value, rolesRequested).Result)
                    result.Result = ClaimResult.Successful;
                else
                    result.Result = ClaimResult.Forbidden;

                return result;
            }

            return result;
        }

        internal class ClaimResults
        {
            public ClaimResult Result { get; set; }
            public string Name { get; set; }

            public Claim IdClaim { get; set; }
        }

        internal enum ClaimResult
        {
            Successful = 5,
            Forbidden = 10,
            Failed = 15
        }
    }
}
