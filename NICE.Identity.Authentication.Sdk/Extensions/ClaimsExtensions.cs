using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NICE.Identity.Authentication.Sdk.Extensions
{
	public static class ClaimsExtensions
	{
		/// <summary>
		/// This name-identifier is the id, for use by authenticating parties.
		/// It's not IdAM own internal user id - which is only for use by IdAM.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string NameIdentifier(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.NameIdentifier && c.Issuer.Equals(Issuers.IdAM))?.Value;
		}

		public static string FirstName(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.FirstName && c.Issuer.Equals(Issuers.IdAM))?.Value;
		}

		public static string LastName(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.LastName && c.Issuer.Equals(Issuers.IdAM))?.Value;
		}

		public static string EmailAddress(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.EmailAddress && c.Issuer.Equals(Issuers.IdAM))?.Value;
		}

		public static string DisplayName(this ClaimsPrincipal claimsPrincipal)
		{
			return ($"{FirstName(claimsPrincipal)} {LastName(claimsPrincipal)}").Trim();
		}
		
		public static bool IsStaff(this ClaimsPrincipal claimsPrincipal)
		{
			var isStaffClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.IsStaff && c.Issuer.Equals(Issuers.IdAM))?.Value;
			if (isStaffClaim != null && bool.TryParse(isStaffClaim, out var isStaff))
			{
				return isStaff;
			}
			return false;
		}

		public static IEnumerable<string> Roles(this ClaimsPrincipal claimsPrincipal, string host)
		{
			return claimsPrincipal.Claims.Where(claim => claim.Type.Equals(ClaimType.Role) && claim.Issuer.Equals(host, StringComparison.OrdinalIgnoreCase)).Select(claim => claim.Value);
		}
	}
}
