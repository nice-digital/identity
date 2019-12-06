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
		/// it's composed of a guid with a prefix
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static string NameIdentifier(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.NameIdentifier && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
		}

		public static string FirstName(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.FirstName && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
		}

		public static string LastName(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.LastName && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
		}

		public static string EmailAddress(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.EmailAddress && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
		}

		public static string DisplayName(this ClaimsPrincipal claimsPrincipal)
		{
			return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.DisplayName && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
		}
		
		/// <summary>
		/// the isstaff property is only determined by whether they've signed in via AD.
		/// staff may also sign in via other means (password or google) and this property won't be set correctly for them.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static bool IsStaff(this ClaimsPrincipal claimsPrincipal)
		{
			var isStaffClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.IsStaff && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
			if (isStaffClaim != null && bool.TryParse(isStaffClaim, out var isStaff))
			{
				return isStaff;
			}
			return false;
		}

		/// <summary>
		/// This property determines whether the user account has been migrated from nice accounts.
		/// </summary>
		/// <param name="claimsPrincipal"></param>
		/// <returns></returns>
		public static bool IsMigrated(this ClaimsPrincipal claimsPrincipal)
		{
			var isMigratedClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimType.IsMigrated && c.Issuer.Equals(AuthenticationConstants.IdAMIssuer))?.Value;
			if (isMigratedClaim != null && bool.TryParse(isMigratedClaim, out var isMigrated))
			{
				return isMigrated;
			}
			return false;
		}

		public static IEnumerable<string> Roles(this ClaimsPrincipal claimsPrincipal, string host)
		{
			return claimsPrincipal.Claims.Where(claim => claim.Type.Equals(ClaimType.Role) && claim.Issuer.Equals(host, StringComparison.OrdinalIgnoreCase)).Select(claim => claim.Value);
		}
	}
}
