using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Models;
using NICE.Identity.Authorisation.WebAPI.Models.Requests;
using NICE.Identity.Authorisation.WebAPI.Models.Responses;
using Claim = NICE.Identity.Authorisation.WebAPI.Models.Responses.Claim;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Models.Responses.Claim> GetClaims(int userId);
	}

	public class ClaimsService : IClaimsService
	{
		private readonly IdentityContext _context;

		public ClaimsService(IdentityContext context)
		{
			_context = context;
		}
		public List<Models.Responses.Claim> GetClaims(int userId)
		{
			var claims = new List<Claim>();
			var userRoles = _context.GetClaims(userId);

			claims.Add(new Claim(ClaimType.FirstName, userRoles.FirstOrDefault().User.FirstName));

			foreach (var userRole in userRoles)
			{
				claims.Add(new Claim(ClaimType.Role, userRole.Role.Name));
			}

			return claims;
		}
	}
}
