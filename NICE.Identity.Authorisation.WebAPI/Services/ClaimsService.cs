﻿using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authorisation.WebAPI.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Claim = NICE.Identity.Authorisation.WebAPI.ApiModels.Claim;
using IdentityContext = NICE.Identity.Authorisation.WebAPI.Repositories.IdentityContext;

namespace NICE.Identity.Authorisation.WebAPI.Services
{
	public interface IClaimsService
	{
		List<Claim> GetClaims(string authenticationProviderUserId);
	}

	public class ClaimsService : IClaimsService
	{
		private readonly IdentityContext _context;
	    private readonly ILogger<ClaimsService> _logger;
	    private readonly IUsersService _usersService;

	    public ClaimsService(IdentityContext context, ILogger<ClaimsService> logger, IUsersService usersService)
	    {
	        _context = context ?? throw new ArgumentNullException(nameof(context));
	        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
	        _usersService = usersService;
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

		    if (user.IsLockedOut)
		    {
			    //it shouldn't be possible to hit this method if the user is locked out. 
				_logger.LogError($"GetUser hit with locked out user. authenticationProviderUserId: '{authenticationProviderUserId}'");
				throw new Exception("User is locked out"); 
		    }

			claims.Add(new Claim(ClaimType.IdAMId, user.UserId.ToString(), AuthenticationConstants.IdAMIssuer));
			claims.Add(new Claim(ClaimType.NameIdentifier, user.NameIdentifier, AuthenticationConstants.IdAMIssuer));
			claims.Add(new Claim(ClaimType.IsMigrated, user.IsMigrated.ToString(), AuthenticationConstants.IdAMIssuer));
			claims.Add(new Claim(ClaimType.FirstName, user.FirstName, AuthenticationConstants.IdAMIssuer));
		    claims.Add(new Claim(ClaimType.LastName, user.LastName, AuthenticationConstants.IdAMIssuer));
		    claims.Add(new Claim(ClaimType.DisplayName, $"{user.FirstName} {user.LastName}".Trim(), AuthenticationConstants.IdAMIssuer));
			claims.Add(new Claim(ClaimType.EmailAddress, user.EmailAddress, AuthenticationConstants.IdAMIssuer));
		    if (user.IsStaffMember)
		    {
			    claims.Add(new Claim(ClaimType.IsStaff, true.ToString(), AuthenticationConstants.IdAMIssuer));
		    }

		    foreach (var userRole in user.UserRoles)
			{
				claims.Add(new Claim(ClaimType.Role, userRole.Role.Name, userRole.Role.Website.Host));
			}

		    if (user.Jobs.Any())
		    {
			    var organisations = user.Jobs.Select(job => new Authentication.Sdk.Domain.Organisation(job.Organisation.OrganisationId, job.Organisation.Name, job.IsLead)).ToList();
			    claims.Add(new Claim(ClaimType.Organisations, JsonSerializer.Serialize(organisations), AuthenticationConstants.IdAMIssuer));
		    }

		    var latv = user.LatestAcceptedTermsVersion();
            if (latv != null) claims.Add(new Claim(ClaimType.TermsAndConditions, latv.TermsVersionId.ToString(), AuthenticationConstants.IdAMIssuer));

            return claims;
		}
	}
}