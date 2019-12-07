using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Claim = NICE.Identity.Authentication.Sdk.Domain.Claim;

namespace NICE.Identity.Authentication.Sdk.Authorisation
{
	internal static class ClaimsHelper
	{
		internal static async Task AddClaimsToUser(IAuthConfiguration authConfiguration, string userId, string accessToken, string host, ClaimsPrincipal claimsPrincipal, HttpClient client)
		{
			var uri = new Uri($"{authConfiguration.WebSettings.AuthorisationServiceUri}{Constants.AuthorisationURLs.GetClaims}{userId}");

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			var responseMessage = await client.GetAsync(uri); //call the api to get all the claims for the current user
			if (responseMessage.IsSuccessStatusCode)
			{
				var allClaims = JsonConvert.DeserializeObject<Claim[]>(await responseMessage.Content.ReadAsStringAsync());
				//add in all the claims from retrieved from the api, excluding roles where the host doesn't match the current.
				var claimsToAdd = allClaims.Where(claim => (!claim.Type.Equals(ClaimType.Role)) ||
				                                           (claim.Type.Equals(ClaimType.Role) &&
				                                            claim.Issuer.Equals(host, StringComparison.OrdinalIgnoreCase)))
					.Select(claim => new System.Security.Claims.Claim(claim.Type, claim.Value, null, claim.Issuer)).ToList();

				if (claimsToAdd.Any())
				{
					claimsPrincipal.AddIdentity(new ClaimsIdentity(claimsToAdd, null, ClaimType.DisplayName, ClaimType.Role));
				}
			}
			else
			{
				throw new Exception($"Error {(int)responseMessage.StatusCode} trying to set claims when signing in to uri: {uri} using access token: {accessToken}"); //TODO: remove access token from error message.
			}
		}
	}
}
