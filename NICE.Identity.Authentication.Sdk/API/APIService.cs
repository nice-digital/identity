#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NICE.Identity.Authentication.Sdk.API
{
	public interface IAPIService
	{
		Task<IEnumerable<UserDetails>> FindUsers(IEnumerable<string> nameIdentifiers, HttpClient httpClient = null);
		Task<Dictionary<string, IEnumerable<string>>> FindRoles(IEnumerable<string> nameIdentifiers, string host, HttpClient httpClient = null);
		Task<IEnumerable<Organisation>> GetOrganisations(IEnumerable<int> organisationIds, HttpClient httpClient = null);
	}

	public class APIService : IAPIService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly string _authorisationServiceUri;

		public APIService(IHttpContextAccessor httpContextAccessor, IAuthConfiguration authConfiguration)
		{
			_httpContextAccessor = httpContextAccessor;
			_authorisationServiceUri = authConfiguration.WebSettings.AuthorisationServiceUri;
		}

		/// <summary>
		/// Find users
		/// </summary>
		/// <param name="nameIdentifiers"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public async Task<IEnumerable<UserDetails>> FindUsers(IEnumerable<string> nameIdentifiers, HttpClient httpClient = null)
		{
			if (!nameIdentifiers.Any())
				return new List<UserDetails>();

			var pathAndQuery = Constants.AuthorisationURLs.FindUsersFullPath;
			var serialisedNameIdentifiers = JsonConvert.SerializeObject(nameIdentifiers);

			return await PostToAPI<IEnumerable<UserDetails>>(pathAndQuery, serialisedNameIdentifiers, httpClient);
		}

		/// <summary>
		/// Find Roles
		/// </summary>
		/// <param name="nameIdentifiers"></param>
		/// <param name="host"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public async Task<Dictionary<string, IEnumerable<string>>> FindRoles(IEnumerable<string> nameIdentifiers, string host, HttpClient httpClient = null)
		{
			if (!nameIdentifiers.Any())
				return new Dictionary<string, IEnumerable<string>>();

			var pathAndQuery = $"{Constants.AuthorisationURLs.FindRolesFullPath}{WebUtility.UrlEncode(host)}";
			var serialisedNameIdentifiers = JsonConvert.SerializeObject(nameIdentifiers);
			
			return await PostToAPI<Dictionary<string, IEnumerable<string>>>(pathAndQuery, serialisedNameIdentifiers, httpClient);
		}

		/// <summary>
		/// GetOrganisations - This is called by comment collection, which stores OrganisationId's in it's database, but not the Organisation name.
		/// </summary>
		/// <param name="organisationIds"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		public async Task<IEnumerable<Organisation>> GetOrganisations(IEnumerable<int> organisationIds, HttpClient httpClient = null)
		{
			if (!organisationIds.Any())
				return new List<Organisation>();

			var pathAndQuery = Constants.AuthorisationURLs.GetOrganisationsFullPath;
			var serialisedOrganisationIds = JsonConvert.SerializeObject(organisationIds);

			return await PostToAPI<IEnumerable<Organisation>>(pathAndQuery, serialisedOrganisationIds, httpClient);
		}


		/// <summary>
		/// Private helper method to do the heavy lifting for the above calls.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pathAndQuery"></param>
		/// <param name="serialisedObjectToPost"></param>
		/// <param name="httpClient"></param>
		/// <returns></returns>
		private async Task<T> PostToAPI<T>(string pathAndQuery, string serialisedObjectToPost, HttpClient httpClient = null)
		{
			var httpContext = _httpContextAccessor.HttpContext;
			var accessToken = await httpContext.GetTokenAsync("access_token");
			if (string.IsNullOrEmpty(accessToken))
				throw new Exception("Access token not found");

			var client = httpClient ?? new HttpClient();
			var uri = new Uri($"{_authorisationServiceUri}{pathAndQuery}");

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
			{
				Content = new StringContent(serialisedObjectToPost, Encoding.UTF8, "application/json"),
				Headers = {
					Authorization = new AuthenticationHeaderValue(AuthenticationConstants.JWTAuthenticationScheme, accessToken)
				}
			};
			httpRequestMessage.Headers.Add(AuthenticationConstants.HeaderForAddingAllRolesForWebsite, httpContext.Request.Host.Host);

			var responseMessage = await client.SendAsync(httpRequestMessage); //call the api to get all the claims for the current user
			if (responseMessage.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());
			}
			throw new Exception($"Error calling the API. status code: {(int)responseMessage.StatusCode}");
		}
	}
}
#endif