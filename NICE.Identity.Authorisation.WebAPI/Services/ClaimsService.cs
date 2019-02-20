using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using Claim = NICE.Identity.Authorisation.WebAPI.ApiModels.Responses.Claim;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Claim> GetClaims(string authenticationProviderUserId);
		Task AddToUser(Roles role);
	}

	public class ClaimsService : IClaimsService
	{
		private readonly IdentityContext _context;
	    private readonly ILogger<ClaimsService> _logger;

	    public ClaimsService(IdentityContext context, ILoggerFactory loggerFactory)
	    {
	        _context = context ?? throw new ArgumentNullException(nameof(context));

	        if (loggerFactory == null)
	        {
	            throw new ArgumentNullException(nameof(loggerFactory));
	        }

	        _logger = loggerFactory.CreateLogger<ClaimsService>();
	    }

	    public List<Claim> GetClaims(string authenticationProviderUserId)
	    {
	        Users user;

			var claims = new List<Claim>();
            
		    try
		    {
		        user = _context.GetUser(authenticationProviderUserId);
            }
		    catch (Exception e)
		    {
		        _logger.LogError($"GetUser failed - exception: '{e.Message}' authenticationProviderUserId: '{authenticationProviderUserId}'");

                throw new Exception("Failed to get user");
		    }
            
		    if (user == null)
		    {
		        _logger.LogWarning("No users found");

                return null;
		    }

		    claims.Add(new Claim(ClaimType.FirstName, user.FirstName));
		    
			foreach (var userRole in user.UserRoles)
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