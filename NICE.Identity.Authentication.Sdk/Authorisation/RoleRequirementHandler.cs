#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly IHttpClientFactory _httpClientFactory;
	    private readonly IAuthConfiguration _authConfiguration;

	    public RoleRequirementHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IAuthConfiguration authConfiguration)
	    {
		    _httpContextAccessor = httpContextAccessor;
		    _httpClientFactory = httpClientFactory;
		    _authConfiguration = authConfiguration;
	    }

	    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
			if (!context.User.Identity.IsAuthenticated)
			{
				//context.Fail();
				return;
	        }

			//if the granttype is client-credentials, then always succeed. this just ignores the roles on the authorize attribute just for M2M access token, where they've already used the client_secret
			//if we want to make it more complicated later, we can set up scopes for different apis. as it is though, this makes all api calls equal.
			//the equivalent .net framework code is in Http.AuthoriseAttribute.cs
			if (context.User.GrantType().Equals(AuthenticationConstants.ClientCredentials))
			{
				context.Succeed(requirement);
				return;
			}

			var rolesRequired = requirement.Role.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(role => role.Trim()).ToList();

			if (rolesRequired.Contains(Policies.API.RolesWithAccessToUserProfilesPlaceholder))
			{
				rolesRequired.Remove(Policies.API.RolesWithAccessToUserProfilesPlaceholder);
				rolesRequired.AddRange(_authConfiguration.RolesWithAccessToUserProfiles);
			}

			//if the user doesn't have any idam claims, then add them. this will happen during M2M auth.
			if (!context.User.Claims.Any(claim => claim.Type.Equals(ClaimType.IdAMId)))
			{
				var userId = context.User.Claims.Single(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
				var authorisationHeader = _httpContextAccessor.HttpContext.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
				var authHeader = AuthenticationHeaderValue.Parse(authorisationHeader);
				var client = _httpClientFactory.CreateClient();

				var request = _httpContextAccessor.HttpContext.Request;
				var hosts = new List<string> {request.Host.Host};
				if (request.Headers.ContainsKey(AuthenticationConstants.HeaderForAddingAllRolesForWebsite))
				{
					hosts.Add(request.Headers[AuthenticationConstants.HeaderForAddingAllRolesForWebsite]);
				}
				var claimsToAdd = await ClaimsHelper.AddClaimsToUser(_authConfiguration, userId, authHeader.Parameter, hosts, client);
				context.User.AddIdentity(new ClaimsIdentity(claimsToAdd, null, ClaimType.DisplayName, ClaimType.Role));
			}

			if (context.User.Claims.Any(claim => claim.Type.Equals(ClaimType.Role) &&
			                                     rolesRequired.Contains(claim.Value, StringComparer.OrdinalIgnoreCase)))
			{
		        context.Succeed(requirement);
				return;
			}
			context.Fail();
		}
    }
}
#endif