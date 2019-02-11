using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.Models;
using NICE.Identity.Authorisation.WebAPI.Models.Requests;
using Claim = NICE.Identity.Authorisation.WebAPI.Models.Responses.Claim;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Models.Responses.Claim> GetClaims(int userId);
	}

	public class ClaimsService
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


			foreach (var userRole in userRoles)
			{
				claims.Add(new Claim(userRole.RoleId, userRole.Role.Name){});
			}

			return claims;
		}
	}
}
