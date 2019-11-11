using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.ApiModels.Responses;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using Claim = NICE.Identity.Authorisation.WebAPI.ApiModels.Responses.Claim;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Claim> GetClaims(string authenticationProviderUserId);
		Task AddToUser(Role role);
	}

	public class ClaimsService : IClaimsService
	{
		private readonly IdentityContext _context;
	    private readonly ILogger<ClaimsService> _logger;

	    public ClaimsService(IdentityContext context, ILogger<ClaimsService> logger)
	    {
	        _context = context ?? throw new ArgumentNullException(nameof(context));
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	    }

	    public List<Claim> GetClaims(string authenticationProviderUserId)
	    {
	        User user;

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

			claims.Add(new Claim(ClaimType.IdAMId, user.UserId.ToString(), Issuers.IdAM));
			claims.Add(new Claim(ClaimType.NameIdentifier, user.Auth0UserId, Issuers.IdAM));
			claims.Add(new Claim(ClaimType.FirstName, user.FirstName, Issuers.IdAM));
		    claims.Add(new Claim(ClaimType.LastName, user.LastName, Issuers.IdAM));
			claims.Add(new Claim(ClaimType.EmailAddress, user.EmailAddress, Issuers.IdAM));
		    if (user.IsStaffMember.HasValue && user.IsStaffMember.Value)
		    {
			    claims.Add(new Claim(ClaimType.IsStaff, true.ToString(), Issuers.IdAM));
		    }

		    foreach (var userRole in user.UserRoles)
			{
				claims.Add(new Claim(ClaimType.Role, userRole.Role.Name, userRole.Role.Website.Host));
			}

            var latv = user.LatestAcceptedTermsVersion();
            if (latv != null) claims.Add(new Claim(ClaimType.TermsAndConditions, latv.TermsVersionId.ToString(), Issuers.IdAM));

            return claims;
		}

		public Task AddToUser(Role role)
		{
			throw new NotImplementedException();
		}
	}
}