using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NICE.Identity.Authorisation.WebAPI.APIModels.Responses;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using Claim = NICE.Identity.Authorisation.WebAPI.APIModels.Responses.Claim;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Claim> GetClaims(int userId);
		Task AddToUser(Roles role);
	}

	public class ClaimsService : IClaimsService
	{
		private readonly IdentityContext _context;

		public ClaimsService(IdentityContext context)
		{
			_context = context;
		}

		//public List<Models.Responses.Claim> GetClaims(int userId)
		//{
		//	var claims = new List<Claim>();
		//	var userRoles = _context.GetClaims(userId);

		//	if (userRoles.Count == 0)
		//	{
		//		//second hit to the DB to get user
		//		var users = _context.GetUser(userId);

		//		if (users.Count == 0)
		//			return null;

		//		foreach (var user in users)
		//		{
		//			claims.Add(new Claim(ClaimType.FirstName, user.FirstName));
		//		}
		//	}
		//	else
		//	{
		//		claims.Add(new Claim(ClaimType.FirstName, userRoles.FirstOrDefault().User.FirstName));

		//		foreach (var userRole in userRoles)
		//		{
		//			claims.Add(new Claim(ClaimType.Role, userRole.Role.Name));
		//		}
		//	}

		//	return claims;
		//}

		public List<Claim> GetClaims(int userId)
		{
			var claims = new List<Claim>();
			var users = _context.GetUser(userId);

			if (users.Count == 0)
				return null; //TODO What should be returned here?

			claims.Add(new Claim(ClaimType.FirstName, users.FirstOrDefault().FirstName)); //will there be more than one user?

			foreach (var userRole in users.FirstOrDefault().UserRoles)
			{
				claims.Add(new Claim(ClaimType.Role, userRole.Role.Name));
			}

			return claims;
		}

		public Task AddToUser(Roles role)
		{
			throw new NotImplementedException();
		}
	}
}
